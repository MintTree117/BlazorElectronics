namespace BlazorElectronics.Server.Models.Users;

public class UserExists
{
    public bool UsernameExists { get; set; } = false;
    public bool EmailExists { get; set; } = false;
}