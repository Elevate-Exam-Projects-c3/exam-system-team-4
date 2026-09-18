using exam_system.Features.Attempts.StartAttempt.DTOs;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Questions.Queries
{
    public record GetAttemptQuestionsQuery(Guid QuizId,Guid AttemptId) :
                        IRequest<RequestResponse<List<AttemptQuestionDto>>>;
}
