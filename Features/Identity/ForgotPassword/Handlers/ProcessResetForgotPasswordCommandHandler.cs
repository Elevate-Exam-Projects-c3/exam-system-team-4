using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.ForgotPassword.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Identity.ForgotPassword.Handlers;

public sealed class ProcessResetForgotPasswordCommandHandler
    : IRequestHandler<ProcessResetForgotPasswordCommand, RequestResponse>
{
    private readonly IGenericRepository<PasswordResetOtp> _otpRepository;
    private readonly PasswordHasher<PasswordResetOtp> _tokenHasher = new();
    private readonly PasswordHasher<ApplicationUser> _passwordHasher = new();

    public ProcessResetForgotPasswordCommandHandler(IGenericRepository<PasswordResetOtp> otpRepository)
    {
        _otpRepository = otpRepository;
    }

    public async Task<RequestResponse> Handle(
        ProcessResetForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var otp = await _otpRepository.Get(x => x.UserId == request.UserId)
            .AsTracking()
            .Include(x => x.User).ThenInclude(user => user.RefreshTokens)
            .OrderByDescending(x => x.ExpiresAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (otp is null || !otp.IsUsed || otp.ResetTokenHash is null ||
            otp.ResetTokenExpiresAt is null || otp.ResetTokenExpiresAt <= DateTime.UtcNow)
        {
            return RequestResponse.Fail("Invalid or expired reset token.");
        }

        var verification = _tokenHasher.VerifyHashedPassword(otp, otp.ResetTokenHash, request.ResetToken);
        if (verification == PasswordVerificationResult.Failed)
        {
            return RequestResponse.Fail("Invalid or expired reset token.");
        }

        var user = otp.User;
        var newPasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
        // Hashing takes time: do not consume a token that expired meanwhile.
        if (otp.ResetTokenExpiresAt <= DateTime.UtcNow)
        {
            return RequestResponse.Fail("Invalid or expired reset token.");
        }

        user.PasswordHash = newPasswordHash;
        user.SecurityStamp = Guid.NewGuid().ToString();
        user.ConcurrencyStamp = Guid.NewGuid().ToString();
        otp.ResetTokenHash = null;
        otp.ResetTokenExpiresAt = null;
        otp.IsUsed = true;
        foreach (var refreshToken in user.RefreshTokens)
        {
            refreshToken.IsRevoked = true;
        }

        // The loaded entities are tracked; TransactionBehavior saves them atomically.
        return RequestResponse.Ok("Password reset successfully. Please log in.");
    }
}
