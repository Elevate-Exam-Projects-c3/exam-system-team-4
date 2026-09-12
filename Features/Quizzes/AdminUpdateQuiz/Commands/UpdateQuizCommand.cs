using exam_system.Features.Quizzes.Shared.Validators;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Quizzes.AdminUpdateQuiz.Commands
{
    public record UpdateQuizCommand(Guid Id,
                                  string Title,
                                  int DurationMinutes,
                                  DateTime StartDate,
                                  DateTime EndDate,
                                  string? Instructions,
                                  int PassScore ,
                                  int? MaxAttempts ) : IRequest<RequestResponse>, IQuizScheduleRequest;


}
