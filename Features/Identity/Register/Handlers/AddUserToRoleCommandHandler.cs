using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.Register.Commands;
using exam_system.Features.Shared;
using exam_system.Helper;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace exam_system.Features.Identity.Register.Handlers;

public sealed class AddUserToRoleCommandHandler
    : IRequestHandler<AddUserToRoleCommand, RequestResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AddUserToRoleCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    public async Task<RequestResponse> Handle(AddUserToRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user is null)
        {
            return RequestResponse.Fail("User not found.", 404);
        }

        if (await _userManager.IsInRoleAsync(user, SD.Student))
        {
            return RequestResponse.Ok("Student role is already assigned.");
        }

        var addToRoleResult = await _userManager.AddToRoleAsync(user, SD.Student);

        if (!addToRoleResult.Succeeded)
        {
            var errors = new Dictionary<string, string[]>
            {
                ["Identity"] = addToRoleResult.Errors.Select(error => error.Description).ToArray()
            };

            return RequestResponse.Fail(
                "Failed to assign the Student role.",400, errors);
        }

        return RequestResponse.Ok("Student role assigned successfully.");
    }
}
