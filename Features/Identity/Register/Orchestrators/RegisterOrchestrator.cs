using exam_system.Features.Identity.Register.Commands;
using exam_system.Features.Identity.Register.Dtos.response;
using exam_system.Features.Identity.Register.Handlers;
using exam_system.Features.Identity.Register.Notification;
using exam_system.Features.Identity.Register.Queries;
using exam_system.Features.Shared;
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
            // -- email exict
            var emailexist =await _mediator.Send(new EmailExistsQuery(request.Email),cancellationToken);

            // 1. Create the application user.

            var createUserResult = await _mediator.Send(new CreateApplicationUserCommand(request.FullName,
                    request.Email,request.Password),cancellationToken);

            if (!createUserResult.Success)
            {
                return RequestResponse<RegisterResponse>.Fail(createUserResult.Message,createUserResult.StatusCode,
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
                return RequestResponse<RegisterResponse>.Fail(addRoleResult.Message,
                    addRoleResult.StatusCode, addRoleResult.Errors);
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
            var emailVerifyResult = await _mediator.Send(
                new CreateEmailVerificationOtpCommand(
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

            // 5. Send the email now, before the transaction commits.
            await _mediator.Publish(
                new SendVerificationOtpNotification(Email: otpData.Email,FullName: user.FullName,Otp: otpData.PlainOtp),
                cancellationToken);

            // 6. Return only public registration data.
            return RequestResponse<RegisterResponse>.Created(new RegisterResponse(Email: user.Email,
                    RequiresEmailConfirmation: true),
                "Account created successfully. Email confirmation is required.");
        }
       

    }
}
