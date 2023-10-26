namespace BlazorElectronics.Server.Models.Users;

public sealed class UserSession
{
    public int SessionId { get; set; }
    public int UserId { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime LastActivityDate { get; set; }
    public bool IsActive { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public byte[] Salt = Array.Empty<byte>();
    public byte[] Hash = Array.Empty<byte>();
}