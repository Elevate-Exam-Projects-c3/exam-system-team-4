using exam_system.Common.Enums;
using exam_system.Domain.Entities.Attempts;
using exam_system.Features.Attempts.SharedRequests.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Attempts.SharedRequests.Handlers
{
    public class GetInProgressQuizAttemptsCountQueryHandler
        : IRequestHandler<GetInProgressQuizAttemptsCountQuery, RequestResponse<int>>
    {
        private readonly IGenericRepository<QuizAttempt> _quizAttemptRepo;

        public GetInProgressQuizAttemptsCountQueryHandler(IGenericRepository<QuizAttempt>quizAttemptRepo)
        {
            _quizAttemptRepo = quizAttemptRepo;
        }

        public async Task<RequestResponse<int>> Handle(GetInProgressQuizAttemptsCountQuery request, CancellationToken cancellationToken)
        {
           var count= await _quizAttemptRepo.CountAsync(cancellationToken,
                                              quizAttempt => quizAttempt.QuizId == request.quizId&&
                                                             quizAttempt.Status==AttemptStatus.InProgress);
            return RequestResponse<int>.Ok(count);
        }
    }
}
