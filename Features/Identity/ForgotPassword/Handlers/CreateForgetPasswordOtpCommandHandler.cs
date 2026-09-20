using System.Globalization;
using System.Security.Cryptography;
using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.ForgotPassword.Commands;
using exam_system.Features.Identity.ForgotPassword.Response;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Identity.ForgotPassword.Handlers
{
    public class CreateForgetPasswordOtpCommandHandler
    : IRequestHandler<CreateForgetPasswordOtpCommand, RequestResponse<CreateForgotPasswordOtpResult>>
    {
        private readonly IGenericRepository<PasswordResetOtp> _otpRepository;
        private readonly PasswordHasher<PasswordResetOtp> _otpHasher = new();

        public CreateForgetPasswordOtpCommandHandler(
            IGenericRepository<PasswordResetOtp> passwordreset)
        {
            _otpRepository = passwordreset;
        }

        public async Task<RequestResponse<CreateForgotPasswordOtpResult>> Handle(CreateForgetPasswordOtpCommand request,
            CancellationToken cancellationToken)
        {
            var latestOtp = await _otpRepository.Get(otp => otp.UserId == request.UserId)
                .OrderByDescending(otp => otp.ExpiresAt)
                .FirstOrDefaultAsync(cancellationToken);

            var now = DateTime.UtcNow;

            if (latestOtp is not null && latestOtp.ExpiresAt.AddMinutes(-10).AddSeconds(30) > now)
            {
                return RequestResponse<CreateForgotPasswordOtpResult>.Ok(null!);
            }

            var previousOtps = await _otpRepository
                .Get(otp =>
                    otp.UserId == request.UserId &&
                    (!otp.IsUsed || otp.ResetTokenHash != null))
                .ToListAsync(cancellationToken);

            foreach (var previousOtp in previousOtps)
            {
                previousOtp.IsUsed = true;
                previousOtp.ResetTokenHash = null;
                previousOtp.ResetTokenExpiresAt = null;
            }

            var plainOtp = RandomNumberGenerator
                .GetInt32(0, 1_000_000)
                .ToString("D6", CultureInfo.InvariantCulture);

            var passwordResetOtp = new PasswordResetOtp
            {
                UserId = request.UserId,
                ExpiresAt = now.AddMinutes(10),
                AttemptCount = 0,
                IsUsed = false,
                ResetTokenHash = null,
                ResetTokenExpiresAt = null
            };

            passwordResetOtp.OtpHash = _otpHasher.HashPassword(passwordResetOtp, plainOtp);

            _otpRepository.Add(passwordResetOtp);

            return RequestResponse<CreateForgotPasswordOtpResult>.Ok(new CreateForgotPasswordOtpResult(plainOtp));

        }
    }
}
