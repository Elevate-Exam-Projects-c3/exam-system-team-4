using System.ComponentModel.DataAnnotations;

namespace exam_system.Features.Identity.Login.View_Models;

public sealed class LoginViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
