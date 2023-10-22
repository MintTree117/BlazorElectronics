using System.ComponentModel.DataAnnotations;

namespace BlazorElectronics.Shared.Inbound.Users;

public sealed class UserLoginRequest_DTO
{
    [Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}