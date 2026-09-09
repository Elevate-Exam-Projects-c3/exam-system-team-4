using exam_system.Common.Enums;
using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Quizzes.AdminUpdateQuiz.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Quizzes.AdminUpdateQuiz.Handlers
{
    public class UpdateQuizHandler : IRequestHandler<UpdateQuizCommand, RequestResponse>
    {
        private readonly IGenericRepository<Quiz> _quizRepo;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateQuizHandler(IGenericRepository<Quiz> quizRepo, IUnitOfWork unitOfWork)
        {
            _quizRepo = quizRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<RequestResponse> Handle(UpdateQuizCommand request, CancellationToken cancellationToken)
        {
            //Validate dates
            //
            if (request.StartDate < DateTime.UtcNow || request.EndDate <= DateTime.UtcNow)
            {
                var errors = new Dictionary<string, string[]>();
                if (request.StartDate < DateTime.UtcNow)
                {
                    errors.Add("StartDate", ["Start date must be in the future."]);
                }
                if (request.EndDate <= DateTime.UtcNow)
                {
                    errors.Add("EndDate", ["End date must be after start date."]);
                }
                return RequestResponse.Fail(
                                 "Invalid quiz dates.",
                                  400,
                                  errors);
            }
            if (IsEndDateBeforeOrEqualToStartDate(request.StartDate, request.EndDate))
            {

                return RequestResponse.Fail(
                        "End date must be after start date.",
                        400,
                        new Dictionary<string, string[]>
                        {
                             { "EndDate", ["End date must be after start date."] }
                        });
            }
            //
            if (IsDurationMinutesExceedingDateRange(request.StartDate, request.EndDate, request.DurationMinutes))
            {
                return RequestResponse.Fail(
                        "Duration cannot exceed the time between start date and end date.",
                        400,
                        new Dictionary<string, string[]>
                        {
                             { "DurationMinutes", ["Duration cannot exceed the time between start date and end date."] }
                        });
            }
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

            _quizRepo.Update(quiz);
            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
            if (result > 0)
                return RequestResponse.Ok();
            else
                return RequestResponse.Fail("Failed to Update Quiz ", 500);
        }

        //
        private bool IsEndDateBeforeOrEqualToStartDate(DateTime startDate, DateTime endDate)
        {
            return endDate <= startDate;
        }
        private bool IsDurationMinutesExceedingDateRange(DateTime startDate, DateTime endDate, int durationMinutes)
        {
            return (endDate - startDate).TotalMinutes < durationMinutes; ;
        }
    }
}
