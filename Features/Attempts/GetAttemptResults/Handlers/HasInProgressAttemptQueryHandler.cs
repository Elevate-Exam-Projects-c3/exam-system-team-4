using exam_system.Common.Enums;
using exam_system.Domain.Entities.Attempts;
using exam_system.Features.Attempts.GetAttemptResults.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Attempts.GetAttemptResults.Handlers
{
    public class HasInProgressAttemptQueryHandler(IGenericRepository<QuizAttempt> repository) : IRequestHandler<HasInProgressAttemptQuery, RequestResponse<bool>>
    {
        public async Task<RequestResponse<bool>> Handle(HasInProgressAttemptQuery request, CancellationToken cancellationToken)
        {
            var hasInProgressAttempt = await  repository.GetAll().AnyAsync(q => q.QuizId == request.QuizId && q.Status == AttemptStatus.InProgress,cancellationToken);

            return RequestResponse<bool>.Ok(hasInProgressAttempt);
        }
    }
}
