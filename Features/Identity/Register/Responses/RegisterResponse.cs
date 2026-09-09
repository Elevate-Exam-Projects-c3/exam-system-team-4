using exam_system.Features.Shared;
using exam_system.Features.Shared.Cqrs;

namespace exam_system.Features.Identity.Register.Responses
{
    public class RegisterResponse
    {

        public string Email { get; init; } = string.Empty;

        public bool EmailConfirmed { get; init; }
    }
}
