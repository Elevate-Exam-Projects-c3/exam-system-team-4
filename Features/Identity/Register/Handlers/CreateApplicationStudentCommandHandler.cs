using exam_system.Domain.Entities.Identity;
using exam_system.Features.Identity.Register.Commands;
using exam_system.Features.Identity.Register.Dtos.response;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Identity.Register.Handlers;

public sealed class CreateStudentCommandHandler
    : IRequestHandler<CreateStudentCommand,RequestResponse<StudentResponse>>
{
    private readonly IGenericRepository<Student> _studentRepository;

    public CreateStudentCommandHandler(
        IGenericRepository<Student> studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<RequestResponse<StudentResponse>> Handle( CreateStudentCommand request,CancellationToken cancellationToken)
    {
       

        var student = new Student
        {
            UserId = request.id,
        };

        _studentRepository.Add(student);

        var response = new StudentResponse
        {
            Userid = student.UserId
        };

        if (string.IsNullOrWhiteSpace(request.id))
        {
            var errors = new Dictionary<string, string[]>
            {
                ["UserId"] = ["UserId is required."]
            };

            return RequestResponse<StudentResponse>.Fail(
                message: "Failed to create student.",
                statusCode: 400,
                errors: errors);
        }
        return RequestResponse<StudentResponse>.Ok(
            response,
            "Student creation step completed.");
    }
}