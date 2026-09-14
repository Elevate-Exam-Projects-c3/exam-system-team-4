using exam_system.Domain.Entities.Identity;
using exam_system.Features.Shared.UserLookup.Queries;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace exam_system.Features.Shared.UserLookup.Handlers;

public sealed class FindUserByEmailQueryHandler
    : IRequestHandler<FindUserByEmailQuery, RequestResponse<ApplicationUser?>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public FindUserByEmailQueryHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<RequestResponse<ApplicationUser?>> Handle(
        FindUserByEmailQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await _userManager.FindByEmailAsync(request.Email.Trim());

        return RequestResponse<ApplicationUser?>.Ok(user);
    }
}
