namespace BlazorElectronics.Shared.Users;

public class UserRequest
{
    public UserRequest()
    {
        
    }

    public UserRequest( UserSessionResponse session )
    {
        SessionId = session.SessionId;
        SessionToken = session.SessionToken;
    }
    
    public UserRequest( int sessionId, string token )
    {
        SessionId = sessionId;
        SessionToken = token;
    }
    
    public int SessionId { get; protected init; }
    public string SessionToken { get; protected init; } = string.Empty;
}