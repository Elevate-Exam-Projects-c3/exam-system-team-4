using exam_system.Features.Identity.Register.Dtos.response;
using exam_system.Features.Shared;
using exam_system.Features.Shared.Cqrs;

namespace exam_system.Features.Identity.Register.Commands;

public record CreateStudentCommand(string id): ICommand<RequestResponse<StudentResponse>>;
