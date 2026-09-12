using exam_system.Common.Enums;
using exam_system.Domain.Entities.Attempts;
using exam_system.Features.Attempts.StartAttempt.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Attempts.StartAttempt.Handlers
{
    public class GetSubmittedAndTimedOutAttemptsCountQueryHandler : IRequestHandler<GetSubmittedAndTimedOutAttemptsCountQuery, RequestResponse<int>>
    {
        private readonly IGenericRepository<QuizAttempt> _quizAttempRepo;

        public GetSubmittedAndTimedOutAttemptsCountQueryHandler(IGenericRepository<QuizAttempt> quizAttempRepo)
        {
            _quizAttempRepo = quizAttempRepo;
        }

        public async Task<RequestResponse<int>> Handle(GetSubmittedAndTimedOutAttemptsCountQuery request, CancellationToken cancellationToken)
        {
            var count= await _quizAttempRepo.CountAsync(cancellationToken,
                                attempt => attempt.StudentId == request.StudentId &&
                                attempt.QuizId == request.QuizId &&

                               (attempt.Status == AttemptStatus.Submitted || 
                                attempt.Status == AttemptStatus.TimedOut));
            return RequestResponse<int>.Ok(count);
        }
    }
}
