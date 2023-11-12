namespace BlazorElectronics.Server.Dtos.Users;

public sealed class UserDeviceInfoDto
{
    public UserDeviceInfoDto( string? ipAddress )
    {
        IpAddress = ipAddress;
    }
    
    public string? IpAddress { get; }
}