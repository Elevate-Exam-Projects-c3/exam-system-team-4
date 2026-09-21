using exam_system.Common.Enums;
using exam_system.Features.Attempts.SubmitAttempt.DTOs;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Attempts.SubmitAttempt.Commands
{
    public record SubmitQuizAttemptCommand(
                                            Guid AttemptId,
                                            AttemptStatus AttemptStatus,
                                            decimal Score,
                                            bool Passed,
                                            DateTime SubmittedAt
                                        ) : IRequest<RequestResponse>;
}
