namespace BlazorElectronics.Shared.Outbound.Users;

public sealed class UserLoginResponse
{
    public UserLoginResponse()
    {
        
    }

    public UserLoginResponse( string username, string email, string token, bool isAdmin = false )
    {
        Username = username;
        Email = email;
        SessionToken = token;
        IsAdmin = isAdmin;
    }
    
    public string Username { get; } = string.Empty;
    public string Email { get; } = string.Empty;
    public string SessionToken { get; } = string.Empty;
    public bool IsAdmin { get; set; } = false;
}