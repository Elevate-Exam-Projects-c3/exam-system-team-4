using exam_system.Features.Attempts.StartAttempt.DTOs;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Questions.Queries
{
    public record GetQuizQuestionsForNewAttemptQuery(Guid QuizId):
                        IRequest<RequestResponse<List<AttemptQuestionDto>>>;
    
    
}
