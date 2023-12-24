namespace BlazorElectronics.Shared.Users;

public sealed class AccountDetailsDto
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; }
}