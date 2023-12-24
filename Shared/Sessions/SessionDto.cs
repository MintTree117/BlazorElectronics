namespace BlazorElectronics.Shared.Sessions;

public sealed class SessionDto
{
    public string Username { get; set; } = string.Empty;
    public int SessionId { get; init; }
    public string SessionToken { get; init; } = string.Empty;
    public bool IsAdmin { get; init; } = false;
}