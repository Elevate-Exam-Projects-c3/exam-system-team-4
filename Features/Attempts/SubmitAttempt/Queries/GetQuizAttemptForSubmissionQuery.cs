using exam_system.Features.Attempts.SubmitAttempt.DTOs;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Attempts.SubmitAttempt.Queries
{
    public record GetQuizAttemptForSubmissionQuery(Guid AttemptId) : IRequest<RequestResponse<QuizAttemptForSubmissionDto>>;
   
}
