using exam_system.Domain.Entities.Identity;
using exam_system.Features.Shared.UserLookup.Queries;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace exam_system.Features.Shared.UserLookup.Handlers;

public sealed class GetUserRolesQueryHandler
    : IRequestHandler<GetUserRolesQuery, RequestResponse<IReadOnlyList<string>?>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetUserRolesQueryHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<RequestResponse<IReadOnlyList<string>?>> Handle(
        GetUserRolesQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await _userManager.FindByIdAsync(request.UserId);
        IReadOnlyList<string>? roles = user is null
            ? null
            : (await _userManager.GetRolesAsync(user)).ToArray();

        return RequestResponse<IReadOnlyList<string>?>.Ok(roles);
    }
}
