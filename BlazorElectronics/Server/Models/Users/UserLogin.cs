namespace BlazorElectronics.Server.Models.Users;

public sealed class UserLogin
{
    public UserLogin( int id, string name )
    {
        UserId = id;
        Username = name;
    }
    
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
}