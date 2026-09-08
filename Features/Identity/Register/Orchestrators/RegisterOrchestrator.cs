using exam_system.Common.Enums;
using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.Register.Commands;
using exam_system.Features.Identity.Register.Responses;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace exam_system.Features.Identity.Register.Orchestrators
{
    public class RegisterOrchestrator
     : IRequestHandler<RegisterCommand, ApiResponse<RegisterResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGenericRepository<Student> _students;
        private readonly IGenericRepository<EmailVerificationOtp> _otpRepository;

        public RegisterOrchestrator(
            UserManager<ApplicationUser> userManager,IGenericRepository<Student> studentrepo,
            IGenericRepository<EmailVerificationOtp> otpRepository)
        {
            _userManager = userManager;
            _students = studentrepo;
            _otpRepository = otpRepository;
        }

        public async Task<ApiResponse<RegisterResponse>> Handle(RegisterCommand request,
            CancellationToken cancellationToken)
        {
            // 1) Check duplicate email
            var existingUser =
                await _userManager.FindByEmailAsync(request.Email.Trim());

            if (existingUser is not null)
            {
                return ApiResponse<RegisterResponse>.Fail("Email already registered",409);
            }

            // 2) Create ApplicationUser 
            var user = new ApplicationUser
            {
                FullName = request.FullName.Trim(),

                Email = request.Email.Trim(),

                UserName = request.Email.Trim(),

                EmailConfirmed = false,

                AccountStatus = AccountStatus.Pending
            };
            // create user 

            var createUserResult = await _userManager.CreateAsync(user,request.Password);

            // validate on register user succeded


            if (!createUserResult.Succeeded)
            {
                var errors = createUserResult.Errors.Select(e => e.Description).ToArray();

                return ApiResponse<RegisterResponse>.Fail("Registration failed",400);
            }
            // add role to user 

            var addRoleResult = await _userManager.AddToRoleAsync( user , "Student");

            // validate on role added 

            if (!addRoleResult.Succeeded)
            {
                var errors = createUserResult.Errors.Select(e => e.Description).ToArray();

                return ApiResponse<RegisterResponse>.Fail("Failed to assign Student role", 500);
            }
            // create student 
            var student = new Student
            {
                UserId = user.Id
            };

            _students.Add(student);


               throw new NotImplementedException();
        }
    }
}
