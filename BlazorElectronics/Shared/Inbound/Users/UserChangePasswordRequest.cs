using System.ComponentModel.DataAnnotations;

namespace BlazorElectronics.Shared.Inbound.Users;

public sealed class UserChangePasswordRequest
{
    public SessionApiRequest? ApiRequest { get; set; }

    [Required, StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
    [Compare("Password", ErrorMessage = "Passwords do not match!")]
    public string ConfirmPassword { get; set; } = string.Empty;
}