using BlazorElectronics.Shared.Outbound.Users;

namespace BlazorElectronics.Shared.Inbound.Users;

public sealed class UserDataRequest<T> : UserRequest where T : class
{
    public UserDataRequest()
    {
        
    }

    public UserDataRequest( UserSessionResponse session, T payload )
    {
        SessionId = session.SessionId;
        SessionToken = session.SessionToken;
        Payload = payload;
    }
    
    public UserDataRequest( int sessionId, string token, T payload )
    {
        SessionId = sessionId;
        SessionToken = token;
        Payload = payload;
    }
    
    public T? Payload { get; set; }
}