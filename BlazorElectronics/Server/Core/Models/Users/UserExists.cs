namespace BlazorElectronics.Server.Core.Models.Users;

public class UserExists
{
    public bool UsernameExists { get; set; } = false;
    public bool EmailExists { get; set; } = false;
}