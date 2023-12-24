using System.ComponentModel.DataAnnotations;

namespace BlazorElectronics.Shared.Users;

public sealed class LoginRequestDto
{
    [Required]
    public string EmailOrUsername { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}