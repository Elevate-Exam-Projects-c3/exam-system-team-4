using exam_system.Features.Identity.Register.Commands;
using exam_system.Features.Identity.Register.Dtos.response;
using exam_system.Features.Identity.Register.Notification;
using exam_system.Features.Shared;
using exam_system.Features.Shared.UserLookup.Queries;
using MediatR;

namespace exam_system.Features.Identity.Register.Orchestrators
{
    public class RegisterOrchestrator
    : IRequestHandler<RegisterCommand,RequestResponse<RegisterResponse>>
    {
        private readonly IMediator _mediator;

        public RegisterOrchestrator(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<RequestResponse<RegisterResponse>> Handle(RegisterCommand request,
            CancellationToken cancellationToken)
        {
            // Check whether an account already uses this email.
            var findUserResult = await _mediator.Send( new FindUserByEmailQuery(request.Email),cancellationToken);

            if (!findUserResult.Success)
            {
                return RequestResponse<RegisterResponse>.Fail(
                    findUserResult.Message,
                    findUserResult.StatusCode,
                    findUserResult.Errors);
            }

            if (findUserResult.Data is not null)
            {
                return RequestResponse<RegisterResponse>.Fail(
                    "Email is already registered.",
                    409);
            }

            // 1. Create the application user.

            var createUserResult = await _mediator.Send(new CreateApplicationUserCommand(request.FullName,
                    request.Email,request.Password),cancellationToken);

            if (!createUserResult.Success)
            {
                return RequestResponse<RegisterResponse>.Fail(
                    createUserResult.Message,
                    createUserResult.StatusCode,
                    createUserResult.Errors);
            }

            var user = createUserResult.Data;

            if (user is null)
            {
                return RequestResponse<RegisterResponse>.Fail(
                    "User creation returned no user data.",
                    500);
            }

            // 2. Assign the Student role to the created user.

            var addRoleResult = await _mediator.Send( new AddUserToRoleCommand(user.UserId),cancellationToken);

            if (!addRoleResult.Success)
            {
                return RequestResponse<RegisterResponse>.Fail(
                    addRoleResult.Message,
                    addRoleResult.StatusCode,
                    addRoleResult.Errors);
            }
            // 3. Create the student profile.

            var createStudentResult = await _mediator.Send(new CreateStudentCommand(user.UserId),cancellationToken);

            if (!createStudentResult.Success)
            {
                return RequestResponse<RegisterResponse>.Fail(
                    message: createStudentResult.Message,
                    statusCode: createStudentResult.StatusCode,
                    errors: createStudentResult.Errors);
            }


            // 4. Create the OTP.
            var emailVerifyResult = await _mediator.Send(new CreateEmailVerificationOtpCommand(
                    user.UserId,
                    user.Email),
                cancellationToken);

            if (!emailVerifyResult.Success)
            {
                return RequestResponse<RegisterResponse>.Fail(
                    emailVerifyResult.Message,
                    emailVerifyResult.StatusCode,
                    emailVerifyResult.Errors);
            }

            var otpData = emailVerifyResult.Data;

            if (otpData is null)
            {
                return RequestResponse<RegisterResponse>.Fail(
                    "Verification code creation returned no data.",
                    500);
            }

            // 5. Send the email now, 
            await _mediator.Publish(
                new SendVerificationOtpNotification( otpData.Email,user.FullName,otpData.PlainOtp),cancellationToken);

            // 6. Return only public registration data.
            return RequestResponse<RegisterResponse>.Created(new RegisterResponse(user.Email,true),
                "Account created successfully. Email confirmation is required.");
        }
       

    }
}
