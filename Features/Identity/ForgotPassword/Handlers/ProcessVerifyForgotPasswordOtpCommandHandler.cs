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
    public sealed class ProcessVerifyForgotPasswordOtpCommandHandler
      : IRequestHandler<ProcessVerifyForgotPasswordOtpCommand,RequestResponse<ProcessVerifyForgotPasswordOtpResult>>
    {
        private const int MaxAttempts = 5;

        private readonly IGenericRepository<PasswordResetOtp> _otpRepository;
        private readonly PasswordHasher<PasswordResetOtp> _hasher = new();

        public ProcessVerifyForgotPasswordOtpCommandHandler(
            IGenericRepository<PasswordResetOtp> otpRepository)
        {
            _otpRepository = otpRepository;
        }

        public async Task<RequestResponse<ProcessVerifyForgotPasswordOtpResult>>
            Handle(
                ProcessVerifyForgotPasswordOtpCommand request,
                CancellationToken cancellationToken)
        {
            var otp = await _otpRepository
                .Get(x => x.UserId == request.UserId)
                .OrderByDescending(x => x.ExpiresAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (otp is null)
            {
                return Result(
                    PasswordResetOtpVerificationStatus.InvalidCode);
            }

            if (otp.AttemptCount >= MaxAttempts)
            {
                return Result(
                    PasswordResetOtpVerificationStatus.AttemptsExceeded);
            }

            if (otp.IsUsed)
            {
                return Result(
                    PasswordResetOtpVerificationStatus.InvalidCode);
            }

            if (otp.ExpiresAt <= DateTime.UtcNow)
            {
                return Result(
                    PasswordResetOtpVerificationStatus.CodeExpired);
            }

            var verification = _hasher.VerifyHashedPassword(
                otp,
                otp.OtpHash,
                request.Otp);

            if (verification == PasswordVerificationResult.Failed)
            {
                otp.AttemptCount++;

                if (otp.AttemptCount >= MaxAttempts)
                {
                    otp.IsUsed = true;
                    otp.ResetTokenHash = null;
                    otp.ResetTokenExpiresAt = null;
                }

                _otpRepository.Update(otp);

                return Result(
                    otp.AttemptCount >= MaxAttempts
                        ? PasswordResetOtpVerificationStatus.AttemptsExceeded
                        : PasswordResetOtpVerificationStatus.InvalidCode);
            }

            // Prevent accepting a code that expired during hash verification.
            if (otp.ExpiresAt <= DateTime.UtcNow)
            {
                return Result(
                    PasswordResetOtpVerificationStatus.CodeExpired);
            }

            var resetToken = Convert.ToHexString(
                RandomNumberGenerator.GetBytes(32));

            otp.IsUsed = true;
            otp.ResetTokenHash = _hasher.HashPassword(otp, resetToken);
            otp.ResetTokenExpiresAt = DateTime.UtcNow.AddMinutes(10);

            _otpRepository.Update(otp);

            return Result(
                PasswordResetOtpVerificationStatus.Verified,
                resetToken);
        }

        private static RequestResponse<ProcessVerifyForgotPasswordOtpResult>
            Result(
                PasswordResetOtpVerificationStatus status,
                string? resetToken = null)
        {
            return RequestResponse<ProcessVerifyForgotPasswordOtpResult>.Ok(
                new ProcessVerifyForgotPasswordOtpResult(
                    status,
                    resetToken));
        }
    }
}
