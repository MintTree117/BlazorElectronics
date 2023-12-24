using BlazorElectronics.Shared;

namespace BlazorElectronics.Client.Services.Admin;

public interface IAdminCrudService<Tview,Tedit> where Tview : CrudViewDto
{
    Task<ServiceReply<List<Tview>?>> GetView( string path );
    Task<ServiceReply<Tedit?>> GetEdit( string path, int itemId );
    Task<ServiceReply<int>> Add( string path, Tedit dto );
    Task<ServiceReply<bool>> Update( string path, Tedit dto );
    Task<ServiceReply<bool>> RemoveItem( string path, int itemId );
}