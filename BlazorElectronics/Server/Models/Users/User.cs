namespace BlazorElectronics.Server.Models.Users;

public sealed class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
    public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
    public DateTime DateCreated { get; set; } = DateTime.Now;
    public bool IsAdmin { get; set; } = false;
}