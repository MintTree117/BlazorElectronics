namespace BlazorElectronics.Shared.Outbound.Users;

public sealed class UserLoginResponse_DTO
{
    public string Username { get; set; } = string.Empty;
    public string JsonToken { get; set; } = string.Empty;
}