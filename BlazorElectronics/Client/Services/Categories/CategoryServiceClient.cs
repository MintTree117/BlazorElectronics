using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Categories;

namespace BlazorElectronics.Client.Services.Categories;

public class CategoryServiceClient : CachedClientService<CategoryData>, ICategoryServiceClient
{
    const string API_PATH = "api/category/get";

    public CategoryServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage, 1, "Category Data" ) { }

    public async Task<ServiceReply<CategoryData?>> GetCategories()
    {
        CategoryData? data = await TryGetCachedItem();
        
        if ( data is not null )
            return new ServiceReply<CategoryData?>( data );

        ServiceReply<List<CategoryLightDto>?> reply = await TryGetRequest<List<CategoryLightDto>?>( API_PATH );

        if ( !reply.Success || reply.Data is null )
            return new ServiceReply<CategoryData?>( reply.ErrorType, reply.Message );

        data = new CategoryData( reply.Data );
        await TrySetCachedItem( data );

        return new ServiceReply<CategoryData?>( data );
    }
}