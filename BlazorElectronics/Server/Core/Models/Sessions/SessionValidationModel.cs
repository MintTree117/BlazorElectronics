namespace BlazorElectronics.Server.Core.Models.Sessions;

public sealed class SessionValidationModel
{
    public int UserId { get; set; }
    public byte[] TokenHash { get; set; } = { };
    public byte[] TokenSalt { get; set; } = { };
    public DateTime LastActivityDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsAdmin { get; set; }

    public bool IsValid( int maxHours )
    {
        return ( DateTime.Now - LastActivityDate ).Hours < maxHours;
    }
}