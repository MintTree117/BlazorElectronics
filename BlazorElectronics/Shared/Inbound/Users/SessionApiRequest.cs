namespace BlazorElectronics.Shared.Inbound.Users;

public sealed class SessionApiRequest
{
    public SessionApiRequest()
    {
        
    }
    
    public SessionApiRequest( int sessionId, string token )
    {
        SessionId = sessionId;
        SessionToken = token;
    }
    
    public int SessionId { get; init; }
    public string SessionToken { get; init; } = string.Empty;
}