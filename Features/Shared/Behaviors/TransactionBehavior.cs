using exam_system.Features.Shared.Cqrs;
using exam_system.Features.Shared.PostCommit;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Shared.Behaviors
{
    public sealed class TransactionBehavior<TRequest, TResponse>
     : IPipelineBehavior<TRequest, TResponse>
     where TRequest : ITransactionalCommand<TResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPostCommitDispatcher _postCommitDispatcher;
        private readonly IPostCommitStore _postCommitStore;

        public TransactionBehavior(IUnitOfWork unitOfWork, IPostCommitDispatcher postCommitDispatcher
            ,IPostCommitStore postCommitStore)
        {
            _unitOfWork = unitOfWork;
            _postCommitDispatcher = postCommitDispatcher;
            _postCommitStore = postCommitStore;
        }

        public async Task<TResponse> Handle(
         TRequest request,
         CancellationToken cancellationToken,
         RequestHandlerDelegate<TResponse> next)
        {
            // نبدأ بصندوق فاضي
            _postCommitStore.Clear();
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
                    _postCommitStore.Clear();
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
                // ممنوع إرسال الإيميل عند الفشل
                _postCommitStore.Clear();

                await _unitOfWork.RollbackTransactionAsync(
                    CancellationToken.None);

                throw;
            }
            finally
            {
                await _unitOfWork.EndTransactionAsync();
            }

            // لا نصل هنا إلا بعد نجاح الـ Commit
            await _postCommitDispatcher.DispatchAsync(
                CancellationToken.None);

            return response;
        }
    }
}
