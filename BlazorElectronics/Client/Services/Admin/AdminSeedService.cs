using Blazored.LocalStorage;
using BlazorElectronics.Shared;

namespace BlazorElectronics.Client.Services.Admin;

public sealed class AdminSeedService : AdminServiceClient, IAdminSeedService
{
    public AdminSeedService( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }
    
    public async Task<ServiceReply<bool>> SeedProducts( IntDto amount )
    {
        return await TryUserRequest<IntDto, bool>( "api/AdminSeed/products", amount );
    }
    public async Task<ServiceReply<bool>> SeedUsers( IntDto amount )
    {
        return await TryUserRequest<IntDto, bool>( "api/AdminSeed/users", amount );
    }
}