using exam_system.Common.Enums;
using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.VerifyEmailOtp.Commands;
using exam_system.Features.Identity.VerifyEmailOtp.Dtos.Response;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Identity.VerifyEmailOtp.Handlers;

public sealed class ProcessVerifyEmailOtpCommandHandler
    : IRequestHandler<VerifyEmailOtpCommand, RequestResponse<VerifyEmailOtpResponse>>
{
    private const int MaxAttempts = 5;

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IGenericRepository<EmailVerificationOtp> _otpRepository;
    private readonly IPasswordHasher<EmailVerificationOtp> _otpHasher;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessVerifyEmailOtpCommandHandler(
        UserManager<ApplicationUser> userManager,
        IGenericRepository<EmailVerificationOtp> otpRepository,
        IPasswordHasher<EmailVerificationOtp> otpHasher,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _otpRepository = otpRepository;
        _otpHasher = otpHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<RequestResponse<VerifyEmailOtpResponse>> Handle(
        VerifyEmailOtpCommand request,
        CancellationToken cancellationToken)
    {
        // 1) البحث عن المستخدم والتحقق من حالة الحساب.
        var user = await _userManager.FindByEmailAsync(request.Email.Trim());

        if (user is null)
        {
            return RequestResponse<VerifyEmailOtpResponse>.Fail("Invalid email or verification code.");
        }

        if (user.EmailConfirmed)
        {
            return RequestResponse<VerifyEmailOtpResponse>.Fail("Email is already verified.", 409);
        }

        if (user.AccountStatus != AccountStatus.Pending)
        {
            return RequestResponse<VerifyEmailOtpResponse>.Fail("Account is not pending verification.", 409);
        }

        // 2) جلب أحدث كود فقط، حتى لو كان مستخدمًا.
        var latestOtp = await _otpRepository
            .Get(otp => otp.UserId == user.Id && otp.Email == user.Email)
            .OrderByDescending(otp => otp.CreatedAt)
            .ThenByDescending(otp => otp.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (latestOtp is null || latestOtp.IsUsed)
        {
            return RequestResponse<VerifyEmailOtpResponse>.Fail("No active verification code. Request a new code.");
        }

        // 3) التحقق من انتهاء الصلاحية وعدد المحاولات.
        if (DateTime.UtcNow >= latestOtp.ExpiresAt)
        {
            return RequestResponse<VerifyEmailOtpResponse>.Fail("Verification code expired. Request a new code.", 410);
        }

        if (latestOtp.AttemptCount >= MaxAttempts)
        {
            return RequestResponse<VerifyEmailOtpResponse>.Fail("Verification code is locked. Request a new code.", 423);
        }

        // 4) مطابقة الكود مع الـ hash وحفظ أي محاولة خاطئة.
        var verificationResult = _otpHasher.VerifyHashedPassword(latestOtp,latestOtp.OtpHash,request.Otp);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            latestOtp.AttemptCount++;
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (latestOtp.AttemptCount >= MaxAttempts)
            {
                return RequestResponse<VerifyEmailOtpResponse>.Fail("Verification code is locked. Request a new code.", 423);
            }

            return RequestResponse<VerifyEmailOtpResponse>.Fail(
                $"Invalid verification code. {MaxAttempts - latestOtp.AttemptCount} attempts remaining.");
        }

        // 5) تفعيل الحساب واستهلاك الكود وحفظ التغييرات معًا.
        latestOtp.IsUsed = true;
        user.EmailConfirmed = true;
        user.AccountStatus = AccountStatus.Active;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new VerifyEmailOtpResponse(
            user.Id,
            user.Email!,
            user.EmailConfirmed,
            user.AccountStatus.ToString());

        return RequestResponse<VerifyEmailOtpResponse>.Ok(response, "Email verified successfully.");
    }
}
