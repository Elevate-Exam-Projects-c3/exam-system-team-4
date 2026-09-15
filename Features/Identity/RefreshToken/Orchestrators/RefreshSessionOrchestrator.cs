using exam_system.Common.Enums;
using exam_system.Features.Identity.RefreshTokens.Commands;
using exam_system.Features.Identity.RefreshTokens.Notifications;
using exam_system.Features.Identity.RefreshTokens.Queries;
using exam_system.Features.Shared;
using exam_system.Features.Shared.Services;
using exam_system.Features.Shared.UserLookup.Queries;
using MediatR;

namespace exam_system.Features.Identity.RefreshTokens.Orchestrators;

public sealed class RefreshSessionOrchestrator
    : IRequestHandler<RefreshSessionCommand, RequestResponse<TokenResult>>
{
    private const string InvalidRefreshToken = "Invalid refresh token. Please log in again.";

    private readonly IMediator _mediator;

    public RefreshSessionOrchestrator(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<RequestResponse<TokenResult>> Handle(
        RefreshSessionCommand request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var refreshTokenValue = request.RefreshToken;
        if (string.IsNullOrWhiteSpace(refreshTokenValue))
        {
            return RequestResponse<TokenResult>.Fail(InvalidRefreshToken, 401);
        }

        // 1. Find the stored token and reject any reuse before other validity checks.
        var findTokenResult = await _mediator.Send(
            new FindRefreshTokenQuery(refreshTokenValue), cancellationToken);

        if (!findTokenResult.Success)
        {
            return RequestResponse<TokenResult>.Fail(
                findTokenResult.Message, findTokenResult.StatusCode, findTokenResult.Errors);
        }

        var storedToken = findTokenResult.Data;
        if (storedToken is null)
        {
            return RequestResponse<TokenResult>.Fail(InvalidRefreshToken, 401);
        }

        if (storedToken.IsUsed)
        {
            await _mediator.Publish(
                new RefreshTokenReuseDetectedNotification(storedToken.Id, storedToken.UserId),
                cancellationToken);

            return RequestResponse<TokenResult>.Fail(InvalidRefreshToken, 401);
        }

        if (storedToken.IsRevoked || storedToken.ExpiresAt <= DateTime.UtcNow)
        {
            return RequestResponse<TokenResult>.Fail(InvalidRefreshToken, 401);
        }

        // 2. Refresh only an active, verified account that is not currently locked out.
        var findUserResult = await _mediator.Send(
            new FindUserByIdQuery(storedToken.UserId), cancellationToken);

        if (!findUserResult.Success)
        {
            return RequestResponse<TokenResult>.Fail(
                findUserResult.Message, findUserResult.StatusCode, findUserResult.Errors);
        }

        var user = findUserResult.Data;
        if (user is null ||
            user.AccountStatus != AccountStatus.Active ||
            !user.EmailConfirmed ||
            user.LockoutEnd > DateTimeOffset.UtcNow ||
            string.IsNullOrWhiteSpace(user.Email))
        {
            return RequestResponse<TokenResult>.Fail(InvalidRefreshToken, 401);
        }

        var getRolesResult = await _mediator.Send(
            new GetUserRolesQuery(user.UserId), cancellationToken);

        if (!getRolesResult.Success)
        {
            return RequestResponse<TokenResult>.Fail(
                getRolesResult.Message, getRolesResult.StatusCode, getRolesResult.Errors);
        }

        var roles = getRolesResult.Data;
        if (roles is null || roles.Count == 0)
        {
            return RequestResponse<TokenResult>.Fail(InvalidRefreshToken, 401);
        }

        // 3. Generate replacement tokens using the current roles and student profile.
        var generateTokensResult = await _mediator.Send(
            new GenerateSessionTokensCommand(user.UserId, user.Email, roles, user.StudentId),
            cancellationToken);

        if (!generateTokensResult.Success)
        {
            return generateTokensResult;
        }

        var tokens = generateTokensResult.Data;
        if (tokens is null)
        {
            return RequestResponse<TokenResult>.Fail("Token generation returned no data.", 500);
        }

        // 4. Consume the old token only if it is still valid at the time of the update.
        var markUsedResult = await _mediator.Send(
            new MarkRefreshTokenAsUsedCommand(storedToken.Id, tokens.RefreshToken),
            cancellationToken);

        if (!markUsedResult.Success)
        {
            return RequestResponse<TokenResult>.Fail(
                markUsedResult.Message, markUsedResult.StatusCode, markUsedResult.Errors);
        }

        if (!markUsedResult.Data)
        {
            // A concurrent request may have consumed the token after our first lookup.
            var latestTokenResult = await _mediator.Send(
                new FindRefreshTokenQuery(refreshTokenValue), cancellationToken);

            if (!latestTokenResult.Success)
            {
                return RequestResponse<TokenResult>.Fail(
                    latestTokenResult.Message, latestTokenResult.StatusCode, latestTokenResult.Errors);
            }

            if (latestTokenResult.Data is { IsUsed: true } reusedToken)
            {
                await _mediator.Publish(
                    new RefreshTokenReuseDetectedNotification(reusedToken.Id, reusedToken.UserId),
                    cancellationToken);
            }

            return RequestResponse<TokenResult>.Fail(InvalidRefreshToken, 401);
        }

        // 5. The existing transaction commits this insertion and the old token update together.
        var createTokenResult = await _mediator.Send(
            new CreateRefreshTokenCommand(user.UserId, tokens.RefreshToken, tokens.RefreshTokenExpiresAt),
            cancellationToken);

        if (!createTokenResult.Success)
        {
            return RequestResponse<TokenResult>.Fail(
                createTokenResult.Message, createTokenResult.StatusCode, createTokenResult.Errors);
        }

        return RequestResponse<TokenResult>.Ok(tokens, "Session refreshed successfully.");
    }
}
