using exam_system.Common.Enums;
using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Quizzes.AdminUpdateQuiz.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Quizzes.AdminUpdateQuiz.Handlers
{
    public class UpdateQuizCommandHandler : IRequestHandler<UpdateQuizCommand, RequestResponse>
    {
        private readonly IGenericRepository<Quiz> _quizRepo;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateQuizCommandHandler(IGenericRepository<Quiz> quizRepo, IUnitOfWork unitOfWork)
        {
            _quizRepo = quizRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<RequestResponse> Handle(UpdateQuizCommand request, CancellationToken cancellationToken)
        {
            
            
            var quiz = await _quizRepo.GetByIdAsync(request.Id, cancellationToken);
            if (quiz == null)
            {

                return RequestResponse.Fail($"Quiz with Id {request.Id} is not Found!",
                    404,
                    new Dictionary<string, string[]> { { "QuizId", ["Quiz with Id {request.id} is not Found"] } });
            }
            //checked if published

            if (quiz.Status == QuizStatus.Published)
            {
                return RequestResponse.Fail(
                   $"Quiz with Id {request.Id} cannot be updated because it is published.",
                   409,
                   new Dictionary<string, string[]>
                   {
                          {
                                "QuizId",
                                [$"Quiz with Id {request.Id} cannot be updated because it is published."]
                          }
                   });
            }
            //update 
            quiz.Title = request.Title;
            quiz.DurationMinutes = request.DurationMinutes;
            quiz.Instructions = request.Instructions;
            quiz.MaxAttempts = request.MaxAttempts;
            quiz.PassScore = request.PassScore;
            quiz.UpdatedAt = DateTime.UtcNow;
            quiz.StartDate=request.StartDate;
            quiz.EndDate=request.EndDate;

            _quizRepo.Update(quiz);
            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
            if (result > 0)
                return RequestResponse.Ok();
            else
                return RequestResponse.Fail("Failed to Update Quiz ", 500);
        }

             
    }
}
