using Blazored.LocalStorage;
using BlazorElectronics.Shared;

namespace BlazorElectronics.Client.Services.Admin;

public sealed class AdminCrudService<Tview,Tedit> : AdminServiceClient, IAdminCrudService<Tview,Tedit> where Tview : CrudViewDto where Tedit : class
{
    public AdminCrudService( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }
    
    public async Task<ServiceReply<List<Tview>?>> GetView( string path )
    {
        return await TryUserGetRequest<List<Tview>?>( path );
    }
    public async Task<ServiceReply<Tedit?>> GetEdit( string path, int id )
    {
        return await TryUserGetRequest<Tedit?>( path, GetItemIdParam( id ) );
    }
    public async Task<ServiceReply<int>> Add( string path, Tedit dto )
    {
        return await TryUserPutRequest<int>( path, dto );
    }
    public async Task<ServiceReply<bool>> Update( string path, Tedit dto )
    {
        return await TryUserPostRequest<bool>( path, dto );
    }
    public async Task<ServiceReply<bool>> RemoveItem( string path, int id )
    {
        return await TryUserDeleteRequest<bool>( path, GetItemIdParam( id ) );
    }
}