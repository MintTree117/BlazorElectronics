using System.Text.Json.Serialization;

namespace BlazorElectronics.Shared.Users;

public sealed class UserDataRequestDto<T> : UserRequestDto where T : class
{
    public UserDataRequestDto( SessionReplyDto sessionReply, T payload )
    {
        SessionId = sessionReply.SessionId;
        SessionToken = sessionReply.SessionToken;
        Payload = payload;
    }
    
    [JsonConstructor]
    public UserDataRequestDto( int sessionId, string sessionToken, T payload )
    {
        SessionId = sessionId;
        SessionToken = sessionToken;
        Payload = payload;
    }

    public T Payload { get; init; }
}