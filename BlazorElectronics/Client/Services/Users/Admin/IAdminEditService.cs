using BlazorElectronics.Shared;

namespace BlazorElectronics.Client.Services.Users.Admin;

public interface IAdminEditService<T>
{
    Task<ServiceReply<T?>> GetEdit( IntDto id );
    Task<ServiceReply<T?>> Add( T dto );
    Task<ServiceReply<bool>> Update( T dto );
}