using System.ComponentModel.DataAnnotations;

namespace BlazorElectronics.Shared.Users;

public sealed class PasswordRequestDto
{
    [Required, StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
    [Compare("Password", ErrorMessage = "Passwords do not match!")]
    public string ConfirmPassword { get; set; } = string.Empty;
}