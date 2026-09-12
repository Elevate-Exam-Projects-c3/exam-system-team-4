using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Quizzes.Shared.DTOs;
using exam_system.Features.Quizzes.Shared.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Quizzes.Shared.Handlers
{
    public class GetQuizForStartAttemptQueryHandler : IRequestHandler<GetQuizForStartAttemptQuery, RequestResponse<QuizStartAttemptDto>>
    {
        private readonly IGenericRepository<Quiz> _quizRepo;

        public GetQuizForStartAttemptQueryHandler(IGenericRepository<Quiz> quizRepo)
        {
            _quizRepo = quizRepo;
        }

        public async Task<RequestResponse<QuizStartAttemptDto>> Handle(GetQuizForStartAttemptQuery request, CancellationToken cancellationToken)
        {
            var quiz =await _quizRepo.GetAll().Where(quiz => quiz.Id == request.Id)
                .Select(quiz => new QuizStartAttemptDto
                {
                    DiplomaId = quiz.DiplomaId,
                    QuizId = quiz.Id,
                    StartDate = quiz.StartDate,
                    EndDate = quiz.EndDate,
                    MaxAttempts = quiz.MaxAttempts,
                    DurationMinutes = quiz.DurationMinutes,
                    Status = quiz.Status,
                }).FirstOrDefaultAsync(cancellationToken);
            if (quiz is null)
            {
                return RequestResponse<QuizStartAttemptDto>.Fail($"Quiz with Id: {request.Id} Is not Found",
                                            404,
                                            new Dictionary<string, string[]>{
                                                { "Quiz",[$"Quiz with Id: {request.Id} Is not Found"] }
                                            });
            }
            else
            {
                return RequestResponse<QuizStartAttemptDto>.Ok(quiz);
            }
        }
    }
}
