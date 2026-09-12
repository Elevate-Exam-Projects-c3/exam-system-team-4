using exam_system.Common.Enums;
using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.Login.Commands;
using exam_system.Features.Shared;
using exam_system.Features.Shared.Services;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace exam_system.Features.Identity.Login.Handlers;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, RequestResponse<TokenResult>>
{
    private const int MaxFailedAttempts = 5;
    private const int LockoutMinutes = 15;
    private const string InvalidCredentials = "Invalid email or password";

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IGenericRepository<RefreshToken> _refreshTokens;

    public LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        IGenericRepository<RefreshToken> refreshTokens)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _refreshTokens = refreshTokens;
    }

    public async Task<RequestResponse<TokenResult>> Handle(LoginCommand request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await _userManager.FindByEmailAsync(request.Email.Trim());
        if (user is null)
            return RequestResponse<TokenResult>.Fail(InvalidCredentials, 401);

        var now = DateTimeOffset.UtcNow;
        if (user.LockoutEnd > now)
            return RequestResponse<TokenResult>.Fail("Your account is temporarily locked. Please try again later.", 423);

        if (user.LockoutEnd is not null)
        {
            user.LockoutEnd = null;
            user.AccessFailedCount = 0;
        }

        // 3. Persist bad-password attempts even though the login response is a failure.
        if (!await _userManager.CheckPasswordAsync(user, request.Password))
        {
            user.AccessFailedCount++;
            if (user.AccessFailedCount >= MaxFailedAttempts)
                user.LockoutEnd = now.AddMinutes(LockoutMinutes);

            var failureUpdate = await _userManager.UpdateAsync(user);
            if (!failureUpdate.Succeeded)
                return RequestResponse<TokenResult>.Fail("Unable to update login attempts. Please try again.", 409);

            return RequestResponse<TokenResult>.Fail(InvalidCredentials, 401);
        }

        if (user.AccountStatus == AccountStatus.Pending)
            return RequestResponse<TokenResult>.Fail(
                "Your account is pending email verification. Please verify your email.", 403);

        if (user.AccountStatus != AccountStatus.Active)
            return RequestResponse<TokenResult>.Fail("Your account is not active.", 403);

        if (!user.EmailConfirmed)
            return RequestResponse<TokenResult>.Fail("Please verify your email before logging in.", 403);

        // 5. Create tokens using the roles stored for this account.
        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Count == 0)
            return RequestResponse<TokenResult>.Fail("Your account has no assigned role.", 403);

        var tokens = _tokenService.GenerateTokens(user, roles);
        _refreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = _tokenService.HashRefreshToken(tokens.RefreshToken),
            ExpiresAt = tokens.RefreshTokenExpiresAt.UtcDateTime
        });

        // 6. Identity and the repository share the scoped DbContext. UpdateAsync
        // saves the counter reset and the pending refresh token in one transaction.
        user.AccessFailedCount = 0;
        user.LockoutEnd = null;

        var successUpdate = await _userManager.UpdateAsync(user);
        if (!successUpdate.Succeeded)
            return RequestResponse<TokenResult>.Fail("Unable to complete login. Please try again.", 409);

        return RequestResponse<TokenResult>.Ok(tokens, "Logged in successfully.");
    }
}
