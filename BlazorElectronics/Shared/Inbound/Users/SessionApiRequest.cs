namespace BlazorElectronics.Shared.Inbound.Users;

public sealed class SessionApiRequest
{
    public SessionApiRequest()
    {
        
    }

    public SessionApiRequest( string name, string token )
    {
        Username = name;
        SessionToken = token;
    }
    
    public string Username { get; set; } = string.Empty;
    public string SessionToken { get; set; } = string.Empty;
}