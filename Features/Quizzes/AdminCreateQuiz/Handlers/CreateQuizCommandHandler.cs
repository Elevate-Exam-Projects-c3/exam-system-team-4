using exam_system.Common.Enums;
using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Diplomas.SharedRequests.Queries;
using exam_system.Features.Quizzes.AdminCreateQuiz.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Quizzes.AdminCreateQuiz.Handlers
{
    public class CreateQuizCommandHandler : IRequestHandler<CreateQuizCommand, RequestResponse>
    {
        private readonly IGenericRepository<Quiz> _quizRepo;
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;

        public CreateQuizCommandHandler(IGenericRepository<Quiz> quizRepo, IMediator mediator, IUnitOfWork unitOfWork)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _quizRepo = quizRepo;
        }

        public async Task<RequestResponse> Handle(CreateQuizCommand request, CancellationToken cancellationToken)
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
            //is deploma exist
            var isDiplomaExist = (await _mediator.Send(new CheckDiplomaExistenceById(request.DiplomaId), cancellationToken)).Data;
            if (!isDiplomaExist)
            {
                return RequestResponse.Fail($"Diploma With Id={request.DiplomaId} Is Not Found",
                    404,
                    new Dictionary<string, string[]>
                    {
                        ["DiplomaId"] = [$"Diploma With Id ={request.DiplomaId} Is Not Found "]
                    });
            }

            //create 
            _quizRepo.Add(new Quiz
            {
                DiplomaId = request.DiplomaId,
                Title = request.Title,
                DurationMinutes = request.DurationMinutes,
                Instructions = request.Instructions,
                PassScore = request.PassScore,
                MaxAttempts = request.MaxAttempts,
                Status = QuizStatus.Draft,
                CreatedAt = DateTime.UtcNow
            });
            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
            if (result > 0)
                return RequestResponse.Ok();
            else
                return RequestResponse.Fail("Failed to create => 500 ", 500);

            //
        }
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
