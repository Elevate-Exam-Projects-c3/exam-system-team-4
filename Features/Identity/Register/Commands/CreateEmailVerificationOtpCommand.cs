using exam_system.Features.Identity.Register.Dtos.response;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Identity.Register.Commands;

public record CreateEmailVerificationOtpCommand(string UserId,string Email)
    : IRequest<RequestResponse<EmailVerificationOtpResponse>>;
