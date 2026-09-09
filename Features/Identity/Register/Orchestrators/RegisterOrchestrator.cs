using System.Security.Cryptography;
using exam_system.Common.Enums;
using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.Register.Commands;
using exam_system.Features.Identity.Register.Notification;
using exam_system.Features.Identity.Register.Responses;
using exam_system.Features.Shared;
using exam_system.Features.Shared.PostCommit;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace exam_system.Features.Identity.Register.Orchestrators
{
    public class RegisterOrchestrator
    : IRequestHandler<RegisterCommand,RequestResponse<RegisterResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGenericRepository<Student> _students;
        private readonly IGenericRepository<EmailVerificationOtp> _otpRepository;
        private readonly IPasswordHasher<EmailVerificationOtp> _otpHasher;
        private readonly IPostCommitStore _postCommitStore;

        public RegisterOrchestrator(
            UserManager<ApplicationUser> userManager,IGenericRepository<Student> studentrepo,
            IGenericRepository<EmailVerificationOtp> otpRepository, IPasswordHasher<EmailVerificationOtp> otpHasher,
            IPostCommitStore postCommitStore
            )
        {
            _userManager = userManager;
            _students = studentrepo;
            _otpRepository = otpRepository;
            _otpHasher = otpHasher;
            _postCommitStore = postCommitStore;
        }

        public async Task<RequestResponse<RegisterResponse>> Handle(RegisterCommand request,
            CancellationToken cancellationToken)
        {
            // 1) Check duplicate email
            var existingUser =
                await _userManager.FindByEmailAsync(request.Email.Trim());

            if (existingUser is not null)
            {
                return DuplicateEmail();
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

                return RequestResponse<RegisterResponse>.Fail("Registration failed",400,
                    new Dictionary<string, string[]>
                    {
                        ["identity"]= errors
                    });
            }
            // add role to user 

            var addRoleResult = await _userManager.AddToRoleAsync( user , "Student");

            // validate on role added 

            if (!addRoleResult.Succeeded)
            {
                var errors = addRoleResult.Errors.Select(e => e.Description).ToArray();

                return RequestResponse<RegisterResponse>.Fail("Failed to assign Student role", 500,
                    new Dictionary<string, string[]>
                {
                    ["Identity"] = errors
                });
            }
            // create student 
            var student = new Student
            {
                UserId = user.Id
            };

            _students.Add(student);
            // generate otp 
            var plainOTP = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();

            // Create OTP entity
            var otpEntity = new EmailVerificationOtp
            {
                UserId = user.Id,
                Email = request.Email.Trim(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                AttemptCount = 0,
                IsUsed = false
            };

            // Hash plain OTP
            otpEntity.OtpHash = _otpHasher.HashPassword(otpEntity,plainOTP);

            // Add OTP hash to DbContext
            _otpRepository.Add(otpEntity);
            // store notification 
            _postCommitStore.Add(new SendVerificationOtpNotification(Email:user.Email ,FullName: user.FullName,Otp: plainOTP));
            // respone 
            return RequestResponse<RegisterResponse>.Ok(
                new RegisterResponse
                {
                    Email = user.Email,
                    EmailConfirmed = false
                },
                "Account created. Check your email to verify your account.",
                201);

        }
        private static RequestResponse<RegisterResponse> DuplicateEmail()
        {
            return RequestResponse<RegisterResponse>.Fail(
                "Email already registered",
                409);
        }

    }
}
