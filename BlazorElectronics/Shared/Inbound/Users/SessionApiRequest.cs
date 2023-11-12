namespace BlazorElectronics.Shared.Inbound.Users;

public sealed class SessionApiRequest
{
    public SessionApiRequest()
    {
        
    }

    public SessionApiRequest( string email, string token )
    {
        Email = email;
        SessionToken = token;
    }
    
    public string Email { get; } = string.Empty;
    public string SessionToken { get; } = string.Empty;
}