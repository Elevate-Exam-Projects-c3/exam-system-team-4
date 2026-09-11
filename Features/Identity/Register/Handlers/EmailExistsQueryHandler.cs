using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.Register.Queries;
using exam_system.Features.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace exam_system.Features.Identity.Register.Handlers;

public sealed class EmailExistsQueryHandler
    : IRequestHandler<EmailExistsQuery, RequestResponse<bool>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public EmailExistsQueryHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<RequestResponse<bool>> Handle(
        EmailExistsQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email.Trim());

        return RequestResponse<bool>.Ok(user is not null);
    }
}
