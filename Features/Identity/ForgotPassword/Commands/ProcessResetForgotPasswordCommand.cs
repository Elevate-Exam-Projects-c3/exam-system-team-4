using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Identity.ForgotPassword.Commands;

public sealed record ProcessResetForgotPasswordCommand(
    string UserId, string ResetToken, string NewPassword) : IRequest<RequestResponse>;
