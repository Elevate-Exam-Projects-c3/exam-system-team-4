using exam_system.Features.Shared.Cqrs;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Shared.Behaviors
{
    public sealed class TransactionBehavior<TRequest, TResponse>
     : IPipelineBehavior<TRequest, TResponse>
     where TRequest : ITransactionalCommand<TResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
       

        public TransactionBehavior(IUnitOfWork unitOfWork )
        {
            _unitOfWork = unitOfWork;
            
        }

        public async Task<TResponse> Handle(
         TRequest request,
         CancellationToken cancellationToken,
         RequestHandlerDelegate<TResponse> next)
        {

            // 1. فتح Transaction
            await _unitOfWork.BeginTransactionAsync(
                cancellationToken);
            TResponse response;


            try
            {
                // 2. تشغيل الـ Orchestrator
                 response = await next();

                // 3. التأكد إن النتيجة تحتوي على Success
                if (response is not ITransactionResult result)
                {

                    throw new InvalidOperationException(
                        "Transactional response must implement ITransactionResult.");
                }

                // 4. فشل متوقع من الـ Orchestrator
                if (!result.ShouldCommit)
                {
                    await _unitOfWork.RollbackTransactionAsync(
                        CancellationToken.None);

                    return response;
                }

                // 5. حفظ التغييرات
                await _unitOfWork.SaveChangesAsync(
                    cancellationToken);

                // 6. اعتماد العملية
                await _unitOfWork.CommitTransactionAsync(
                    cancellationToken);

            }
            catch
            {
               

                await _unitOfWork.RollbackTransactionAsync(
                    CancellationToken.None);

                throw;
            }
            finally
            {
                await _unitOfWork.EndTransactionAsync();
            }

           

            return response;
        }
    }
}
