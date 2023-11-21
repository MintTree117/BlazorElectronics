using BlazorElectronics.Shared.Outbound.Users;

namespace BlazorElectronics.Shared.Inbound.Users;

public sealed class UserApiRequest
{
    public UserApiRequest()
    {
        
    }

    public UserApiRequest( UserSessionResponse session, object? data = null )
    {
        SessionId = session.SessionId;
        SessionToken = session.SessionToken;
        Data = data;
    }
    
    public UserApiRequest( int sessionId, string token, object? data = null )
    {
        SessionId = sessionId;
        SessionToken = token;
        Data = data;
    }
    
    public int SessionId { get; init; }
    public string SessionToken { get; init; } = string.Empty;
    public object? Data { get; set; }
}