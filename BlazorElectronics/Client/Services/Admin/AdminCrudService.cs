using Blazored.LocalStorage;
using BlazorElectronics.Shared;

namespace BlazorElectronics.Client.Services.Admin;

public sealed class AdminCrudService<Tview,Tedit> : AdminServiceClient, IAdminCrudService<Tview,Tedit> where Tview : CrudView where Tedit : class
{
    public AdminCrudService( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }
    
    public async Task<ServiceReply<List<Tview>?>> GetView( string path )
    {
        return await TryUserRequest<List<Tview>?>( path );
    }
    public async Task<ServiceReply<Tedit?>> GetEdit( string path, IntDto itemId )
    {
        return await TryUserRequest<IntDto,Tedit?>( path, itemId );
    }
    public async Task<ServiceReply<int>> Add( string path, Tedit dto )
    {
        return await TryUserRequest<Tedit, int>( path, dto );
    }
    public async Task<ServiceReply<bool>> Update( string path, Tedit dto )
    {
        return await TryUserRequest<Tedit, bool>( path, dto );
    }
    public async Task<ServiceReply<bool>> RemoveItem( string path, IntDto itemId )
    {
        return await TryUserRequest<IntDto, bool>( path, itemId );
    }
}