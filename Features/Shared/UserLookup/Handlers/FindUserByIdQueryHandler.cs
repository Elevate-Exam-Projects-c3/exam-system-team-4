using exam_system.Domain.Entities.Identity;
using exam_system.Features.Shared.UserLookup.Dtos.Response;
using exam_system.Features.Shared.UserLookup.Queries;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Shared.UserLookup.Handlers;

public sealed class FindUserByIdQueryHandler
    : IRequestHandler<FindUserByIdQuery, RequestResponse<UserLookupResult?>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public FindUserByIdQueryHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<RequestResponse<UserLookupResult?>> Handle(
        FindUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await _userManager.Users
            .Include(user => user.Student)
            .SingleOrDefaultAsync(user => user.Id == request.UserId, cancellationToken);
        var result = user is null
            ? null
            : new UserLookupResult(
                user.Id,
                user.Email,
                user.AccountStatus,
                user.EmailConfirmed,
                user.LockoutEnd,
                user.Student?.Id);

        return RequestResponse<UserLookupResult?>.Ok(result);
    }
}
