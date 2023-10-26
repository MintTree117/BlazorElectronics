namespace BlazorElectronics.Shared.Outbound.Users;

public sealed class UserLoginResponse
{
    public UserLoginResponse()
    {
        
    }

    public UserLoginResponse( string username, string token )
    {
        Username = username;
        SessionToken = token;
    }
    
    public string Username { get; set; } = string.Empty;
    public string SessionToken { get; set; } = string.Empty;
}