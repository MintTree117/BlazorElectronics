namespace BlazorElectronics.Shared.Users;

public class UserRequestDto
{
    public UserRequestDto()
    {
        
    }

    public UserRequestDto( SessionReplyDto sessionReply )
    {
        SessionId = sessionReply.SessionId;
        SessionToken = sessionReply.SessionToken;
    }
    
    public UserRequestDto( int sessionId, string token )
    {
        SessionId = sessionId;
        SessionToken = token;
    }
    
    public int SessionId { get; init; }
    public string SessionToken { get; init; } = string.Empty;
}