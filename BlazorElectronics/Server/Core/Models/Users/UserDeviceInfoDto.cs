namespace BlazorElectronics.Server.Core.Models.Users;

public sealed class UserDeviceInfoDto
{
    public UserDeviceInfoDto( string? ipAddress )
    {
        IpAddress = ipAddress;
    }
    
    public string? IpAddress { get; }
}