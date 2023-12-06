using System.Text.Json.Serialization;

namespace BlazorElectronics.Shared.Users;

public sealed class UserDataRequest<T> : UserRequest where T : class
{
    public UserDataRequest( UserSessionResponse session, T payload )
    {
        SessionId = session.SessionId;
        SessionToken = session.SessionToken;
        Payload = payload;
    }
    
    [JsonConstructor]
    public UserDataRequest( int sessionId, string sessionToken, T payload )
    {
        SessionId = sessionId;
        SessionToken = sessionToken;
        Payload = payload;
    }

    public T Payload { get; init; }
}