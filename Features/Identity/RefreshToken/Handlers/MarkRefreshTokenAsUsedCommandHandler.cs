using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.RefreshTokens.Commands;
using exam_system.Features.Shared;
using exam_system.Features.Shared.Services;
using exam_system.Persistence.Context;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Identity.RefreshTokens.Handlers;

public sealed class MarkRefreshTokenAsUsedCommandHandler
    : IRequestHandler<MarkRefreshTokenAsUsedCommand, RequestResponse<bool>>
{
    private readonly IGenericRepository<RefreshToken> _refreshTokens;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public MarkRefreshTokenAsUsedCommandHandler(
        IGenericRepository<RefreshToken> refreshTokens,
        ITokenService tokenService, IUnitOfWork unitOfWork)
    {
        _refreshTokens = refreshTokens;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<RequestResponse<bool>> Handle( MarkRefreshTokenAsUsedCommand request,
        CancellationToken cancellationToken)
    {
        var token = await _refreshTokens.GetByIdAsync(request.RefreshTokenId,cancellationToken);

        if (token is null ||token.IsUsed ||token.IsRevoked ||token.ExpiresAt <= DateTime.UtcNow)
        {
            return RequestResponse<bool>.Ok(false);
        }

        token.IsUsed = true;

        token.ReplacedByTokenHash = _tokenService.HashRefreshToken( request.ReplacementRefreshToken);

        token.UpdatedAt = DateTime.UtcNow;

        _refreshTokens.Update(token);

      
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return RequestResponse<bool>.Ok(true);
       
       

    }
}
