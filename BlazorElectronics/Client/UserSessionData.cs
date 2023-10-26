namespace BlazorElectronics.Client;

public sealed class UserSessionData
{
    public UserSessionData( string username, string token )
    {
        Username = username;
        Token = token;
    }
    
    public string Username { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;

    public bool Validate( out string message )
    {
        message = "Valid session data.";
        
        if ( string.IsNullOrWhiteSpace( Username ) )
        {
            message = "No username found!";
            return false;
        }

        if ( string.IsNullOrWhiteSpace( Token ) )
        {
            message = "No token found!";
            return false;
        }

        return true;
    }
}