namespace BlazorElectronics.Server.Core.Models.Users;

public sealed class UserLoginDto
{
    public UserLoginDto( int id, string name, string email, bool isAdmin )
    {
        UserId = id;
        Username = name;
        Email = email;
        IsAdmin = isAdmin;
    }
    
    public int UserId { get; }
    public string Username { get; }
    public string Email { get; }
    public bool IsAdmin { get; }
}