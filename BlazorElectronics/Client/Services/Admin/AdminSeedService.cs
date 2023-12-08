using Blazored.LocalStorage;
using BlazorElectronics.Shared;

namespace BlazorElectronics.Client.Services.Admin;

public sealed class AdminSeedService : AdminServiceClient, IAdminSeedService
{
    const string API_PATH_PRODUCTS = "api/AdminSeed/products";
    const string API_PATH_USERS = "api/AdminSeed/users";
    
    public AdminSeedService( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }
    
    public async Task<ServiceReply<bool>> SeedProducts( IntDto amount )
    {
        return await TryUserRequest<IntDto, bool>( API_PATH_PRODUCTS, amount );
    }
    public async Task<ServiceReply<bool>> SeedUsers( IntDto amount )
    {
        return await TryUserRequest<IntDto, bool>( API_PATH_USERS, amount );
    }
}