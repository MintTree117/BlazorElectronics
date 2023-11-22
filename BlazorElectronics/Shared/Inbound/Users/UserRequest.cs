using BlazorElectronics.Shared.Outbound.Users;

namespace BlazorElectronics.Shared.Inbound.Users;

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
    
    public int SessionId { get; init; }
    public string SessionToken { get; init; } = string.Empty;
}