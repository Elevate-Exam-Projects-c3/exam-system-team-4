using System.Globalization;
using System.Security.Cryptography;
using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.Register.Commands;
using exam_system.Features.Identity.Register.Dtos.response;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace exam_system.Features.Identity.Register.Handlers;

public sealed class CreateEmailVerificationOtpCommandHandler
    : IRequestHandler<CreateEmailVerificationOtpCommand, RequestResponse<EmailVerificationOtpResponse>>
{
    private readonly IGenericRepository<EmailVerificationOtp> _otpRepository;

    private readonly PasswordHasher<EmailVerificationOtp> _otpHasher = new();

    public CreateEmailVerificationOtpCommandHandler(IGenericRepository<EmailVerificationOtp> otpRepository)
       
    {
        _otpRepository = otpRepository;
        
    }

    public async Task<RequestResponse<EmailVerificationOtpResponse>> Handle(
        CreateEmailVerificationOtpCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.UserId) ||
            string.IsNullOrWhiteSpace(request.Email))
        {
            return 
                RequestResponse<EmailVerificationOtpResponse>.Fail(
                    "UserId and email are required.",
                    400);
        }

        var plainotp = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6", CultureInfo.InvariantCulture);

        var otp = new EmailVerificationOtp
        {
            UserId = request.UserId,
            Email = request.Email,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            AttemptCount = 0,
            IsUsed = false
        };
        otp.OtpHash = _otpHasher.HashPassword(otp, plainotp);
        _otpRepository.Add(otp);

        var response = new EmailVerificationOtpResponse
        {
            Email = otp.Email,
            PlainOtp = plainotp,
            ExpiresAt = otp.ExpiresAt
        };

        return RequestResponse<EmailVerificationOtpResponse>.Ok(
            response,
            "Verification code created successfully.",
            200);
    }
}
