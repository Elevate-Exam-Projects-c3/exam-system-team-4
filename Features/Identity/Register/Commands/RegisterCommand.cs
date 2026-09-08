using System.ComponentModel.DataAnnotations;
using exam_system.Features.Identity.Register.Responses;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Identity.Register.Commands
{
    public class RegisterCommand
    : IRequest<ApiResponse<RegisterResponse>>
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
