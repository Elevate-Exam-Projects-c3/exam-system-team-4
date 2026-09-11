namespace exam_system.Features.Identity.Register.ViewModels;

public sealed record RegisterViewModel(
    string FullName,
    string Email,
    string Password);
