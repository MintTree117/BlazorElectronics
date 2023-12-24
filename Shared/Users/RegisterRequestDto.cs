using System.ComponentModel.DataAnnotations;

namespace BlazorElectronics.Shared.Users;

public sealed class RegisterRequestDto
{
    [Required, StringLength(25, MinimumLength = 6)]
    public string Username { get; set; } = string.Empty;
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    [Required, StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
    [Compare("Password", ErrorMessage = "Passwords do not match!")]
    public string ConfirmPassword { get; set; } = string.Empty;
}