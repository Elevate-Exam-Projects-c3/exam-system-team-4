using exam_system.Common.Enums;
using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.Register.Commands;
using exam_system.Features.Identity.Register.Dtos.response;
using exam_system.Features.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;


namespace exam_system.Features.Identity.Register.Handlers;

public sealed class CreateApplicationUserCommandHandler
    : IRequestHandler<CreateApplicationUserCommand, RequestResponse<CreateApplicationUser>>
{
 
    private readonly UserManager<ApplicationUser> _userManager;

    public CreateApplicationUserCommandHandler(
        UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<RequestResponse<CreateApplicationUser>> Handle(CreateApplicationUserCommand request,CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();

        var user = new ApplicationUser
        {
            FullName = request.FullName.Trim(),
            Email = email,
            UserName = email,
            EmailConfirmed = false,
            AccountStatus = AccountStatus.Pending
        };
        var result = await _userManager.CreateAsync(user,request.Password);

        if (!result.Succeeded)
        {
            var errors = new Dictionary<string, string[]>
            {
                ["Identity"] = result.Errors.Select(error => error.Description).ToArray()
            };

            return RequestResponse<CreateApplicationUser>
                .Fail("faild in create user", 400 ,errors);
        }

        var response = new CreateApplicationUser(user.Id,user.FullName,user.Email!);

        return RequestResponse<CreateApplicationUser>.Ok(response, "User created successfully.", 201);

    }
}
