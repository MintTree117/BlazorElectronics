namespace BlazorElectronics.Shared.Outbound.Users;

public sealed class UserSessionResponse
{
    public string SessionToken { get; set; } = string.Empty;
    public bool IsActive { get; set; } = false;
}