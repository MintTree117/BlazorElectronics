namespace BlazorElectronics.Shared.Users;

public sealed class UserSessionResponse
{
    public bool ValidateIntegrity( out string message )
    {
        message = "Valid session data.";

        if ( string.IsNullOrWhiteSpace( Username ) )
        {
            message = "No username found!";
            return false;
        }

        if ( string.IsNullOrWhiteSpace( SessionToken ) )
        {
            message = "No token found!";
            return false;
        }

        return true;
    }
    
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public int SessionId { get; init; }
    public string SessionToken { get; init; } = string.Empty;
    public bool IsAdmin { get; init; } = false;
}