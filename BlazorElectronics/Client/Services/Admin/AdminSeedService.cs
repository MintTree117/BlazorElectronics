using Blazored.LocalStorage;
using BlazorElectronics.Shared;

namespace BlazorElectronics.Client.Services.Admin;

public sealed class AdminSeedService : AdminServiceClient, IAdminSeedService
{
    const string API_ROUTE = "api/AdminSeed";
    const string API_ROUTE_PRODUCTS = $"{API_ROUTE}/products";
    const string API_ROUTE_REVIEWS = $"{API_ROUTE}/reviews";
    const string API_ROUTE_USERS = $"{API_ROUTE}/users";
    
    public AdminSeedService( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }
    
    public async Task<ServiceReply<bool>> SeedProducts( int amount )
    {
        return await TryUserPutRequest<bool>( API_ROUTE_PRODUCTS, amount );
    }
    public async Task<ServiceReply<bool>> SeedReviews( int amount )
    {
        return await TryUserPutRequest<bool>( API_ROUTE_REVIEWS, amount );
    }
    public async Task<ServiceReply<bool>> SeedUsers( int amount )
    {
        return await TryUserPutRequest<bool>( API_ROUTE_USERS, amount );
    }
}