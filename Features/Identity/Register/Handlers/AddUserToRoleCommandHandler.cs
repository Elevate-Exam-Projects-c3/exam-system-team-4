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
            return  RequestResponse.Fail("userId not found", 404);
        }

        var addtorole = await _userManager.AddToRoleAsync(user, SD.Student);

        if (!addtorole.Succeeded)
        {
            var errors = new Dictionary<string, string[]>
            {
                ["identity"]= addtorole.Errors.Select(e=>e.Description).ToArray()
            };

            return RequestResponse.Fail(
                $"Failed to assign role: {errors}",400, errors);
        }

        return RequestResponse.Ok("Student role assigned successfully.", 200);

    }
}
