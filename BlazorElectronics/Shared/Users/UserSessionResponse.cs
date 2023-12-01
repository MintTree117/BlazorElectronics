namespace BlazorElectronics.Shared.Users;

public sealed class UserSessionResponse
{
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public int SessionId { get; init; }
    public string SessionToken { get; init; } = string.Empty;
    public bool IsAdmin { get; init; } = false;
}