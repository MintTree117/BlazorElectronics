using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Products;

namespace BlazorElectronics.Client.Services.Admin;

public sealed class AdminBulkServiceClient : AdminServiceClient, IAdminBulkServiceClient
{
    const string API_PATH_CATEGORIES = "api/AdminCategory/add-bulk";
    const string API_PATH_PRODUCT_KEYS = "api/AdminProduct/add-bulk";
    
    public AdminBulkServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }
    
    public async Task<ServiceReply<bool>> BulkInsertCategories( List<CategoryEdit> categories )
    {
        return await TryUserRequest<List<CategoryEdit>, bool>( API_PATH_CATEGORIES, categories );
    }
    public async Task<ServiceReply<bool>> BulkInsertKeys( ProductKeys keys )
    {
        return await TryUserRequest<ProductKeys, bool>( API_PATH_PRODUCT_KEYS, keys );
    }
}