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
    
    public async Task<ServiceReply<bool>> BulkInsertCategories( List<CategoryEditDtoDto> categories )
    {
        return await TryUserRequest<List<CategoryEditDtoDto>, bool>( API_PATH_CATEGORIES, categories );
    }
    public async Task<ServiceReply<bool>> BulkInsertKeys( ProductKeysDto keysDto )
    {
        return await TryUserRequest<ProductKeysDto, bool>( API_PATH_PRODUCT_KEYS, keysDto );
    }
}