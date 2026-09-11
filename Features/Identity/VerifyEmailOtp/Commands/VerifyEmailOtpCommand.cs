using exam_system.Features.Identity.VerifyEmailOtp.Dtos.Response;
using exam_system.Features.Shared;
using exam_system.Features.Shared.Cqrs;

namespace exam_system.Features.Identity.VerifyEmailOtp.Commands;

public sealed record VerifyEmailOtpCommand(string Email, string Otp)
    : ICommand<RequestResponse<VerifyEmailOtpResponse>>;
