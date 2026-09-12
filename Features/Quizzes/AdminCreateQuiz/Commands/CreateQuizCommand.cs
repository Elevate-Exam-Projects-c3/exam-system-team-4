using exam_system.Domain.Entities.Diplomas;
using exam_system.Features.Quizzes.Shared.Validators;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Quizzes.AdminCreateQuiz.Commands
{
    public record CreateQuizCommand(Guid DiplomaId,
                                  string Title,                             
                                  int DurationMinutes,
                                  DateTime StartDate,
                                  DateTime EndDate,
                                  string? Instructions,
                                  int PassScore = 60,
                                  int? MaxAttempts = null) : IRequest<RequestResponse>, IQuizScheduleRequest;

}
