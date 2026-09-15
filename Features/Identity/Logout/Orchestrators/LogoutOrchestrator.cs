using exam_system.Features.Identity.Logout.Commands;
using exam_system.Features.Identity.Logout.Queries;
using exam_system.Features.Identity.RefreshTokens.Queries;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Identity.Logout.Orchestrators;

public sealed class LogoutOrchestrator : IRequestHandler<LogoutCommand, RequestResponse>
{
    private readonly IMediator _mediator;

    public LogoutOrchestrator(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<RequestResponse> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return RequestResponse.Ok("Logged out successfully.");
        }

        var findTokenResult = await _mediator.Send(
            new FindRefreshTokenQuery(request.RefreshToken), cancellationToken);

        if (!findTokenResult.Success)
        {
            return RequestResponse.Fail(
                findTokenResult.Message, findTokenResult.StatusCode, findTokenResult.Errors);
        }

        var tokenId = findTokenResult.Data?.Id;
        var visitedTokenIds = new HashSet<Guid>();
        while (tokenId.HasValue && visitedTokenIds.Add(tokenId.Value))
        {
            // Revoke before reading the replacement. The transaction holds the update
            // lock, so a competing refresh either finishes first or cannot use this token.
            var revokeResult = await _mediator.Send(
                new RevokeRefreshTokenCommand(tokenId.Value), cancellationToken);

            if (!revokeResult.Success)
            {
                return revokeResult;
            }

            var replacementResult = await _mediator.Send(
                new FindReplacementRefreshTokenIdQuery(tokenId.Value), cancellationToken);

            if (!replacementResult.Success)
            {
                return RequestResponse.Fail(
                    replacementResult.Message, replacementResult.StatusCode, replacementResult.Errors);
            }

            tokenId = replacementResult.Data;
        }

        return RequestResponse.Ok("Logged out successfully.");
    }
}
