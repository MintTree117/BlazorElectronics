using BlazorElectronics.Shared;

namespace BlazorElectronics.Client.Services.Users.Admin;

public interface IAdminViewService<T> where T : AdminItemViewDto
{
    Task<ServiceReply<List<T>?>> GetView();
    Task<ServiceReply<bool>> RemoveItem( IntDto itemId );
}