using exam_system.Domain.Entities.Attempts;
using exam_system.Features.Attempts.SubmitAttempt.DTOs;
using exam_system.Features.Attempts.SubmitAttempt.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Attempts.SubmitAttempt.Handlers
{
    public class GetQuizAttemptForSubmissionQueryHandler :
        IRequestHandler<GetQuizAttemptForSubmissionQuery, 
                        RequestResponse<QuizAttemptForSubmissionDto>>
    {
        private readonly IGenericRepository<QuizAttempt> _quizAttempRepo;

        public GetQuizAttemptForSubmissionQueryHandler(IGenericRepository<QuizAttempt> quizAttempRepo)
        {
            _quizAttempRepo = quizAttempRepo;
        }
        public async Task<RequestResponse<QuizAttemptForSubmissionDto>> Handle(GetQuizAttemptForSubmissionQuery request, CancellationToken cancellationToken)
        {
            var result = await _quizAttempRepo.GetAll()
                                            .AsNoTracking()
                                            .Where(attempt => attempt.Id == request.AttemptId)
                                            .Select(attempt => new QuizAttemptForSubmissionDto
                                            {
                                                AttemptId = attempt.Id,
                                                QuizId = attempt.QuizId,
                                                StudentId = attempt.StudentId,
                                                Status = attempt.Status,
                                                Deadline = attempt.Deadline,
                                                PassScore=attempt.Quiz.PassScore,
                                                
                                                //CorrectAnswersCount=attempt.Answers.Select(answer=>)
                                                                           
                                            }).FirstOrDefaultAsync(cancellationToken);

            if (result is null)
            {
                return RequestResponse<QuizAttemptForSubmissionDto>
                    .Fail("Quiz attempt not found", 404);
            }

            return RequestResponse<QuizAttemptForSubmissionDto>
                .Ok(result);
        }
    }
}
