using System.ComponentModel.DataAnnotations;

namespace BlazorElectronics.Shared.Inbound.Users;

public sealed class UserChangePassword_DTO
{
    [Required, StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
    [Compare("Password", ErrorMessage = "Passwords do not match!")]
    public string ConfirmPassword { get; set; } = string.Empty;
}