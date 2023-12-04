using BlazorElectronics.Shared;

namespace BlazorElectronics.Client.Services.Users.Admin;

public interface IAdminCrudService<Tview,Tedit> where Tview : AdminItemViewDto
{
    Task<ServiceReply<List<Tview>?>> GetView( string path );
    Task<ServiceReply<Tedit?>> GetEdit( string path, IntDto itemId );
    Task<ServiceReply<Tedit?>> Add( string path, Tedit dto );
    Task<ServiceReply<bool>> Update( string path, Tedit dto );
    Task<ServiceReply<bool>> RemoveItem( string path, IntDto itemId );
}