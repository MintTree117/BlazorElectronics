namespace BlazorElectronics.Shared.Sessions;

public sealed class SessionInfoDto
{
    public int SessionId { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime LastActive { get; set; }
    public string IpAddress { get; set; } = string.Empty;
}