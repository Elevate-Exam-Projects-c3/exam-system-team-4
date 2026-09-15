using exam_system.Features.Identity.RefreshTokens.Commands;
using exam_system.Features.Shared;
using exam_system.Features.Shared.Services;
using MediatR;

namespace exam_system.Features.Identity.RefreshTokens.Handlers;

public sealed class GenerateSessionTokensCommandHandler
    : IRequestHandler<GenerateSessionTokensCommand, RequestResponse<TokenResult>>
{
    private readonly ITokenService _tokenService;

    public GenerateSessionTokensCommandHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public Task<RequestResponse<TokenResult>> Handle(
        GenerateSessionTokensCommand request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var tokens = _tokenService.GenerateTokens(
            request.UserId,
            request.Email,
            request.Roles,
            request.StudentId);

        return Task.FromResult(RequestResponse<TokenResult>.Ok(
            tokens,
            "Tokens generated successfully."));
    }
}
