using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Categories;

namespace BlazorElectronics.Client.Services.Categories;

public class CategoryServiceClient : CachedClientService<CategoryDataDto>, ICategoryServiceClient
{
    const string API_PATH = "api/category/get";

    public CategoryServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage, 1, "Category Data" ) { }

    public async Task<ServiceReply<CategoryDataDto?>> GetCategories()
    {
        CategoryDataDto? data = await TryGetCachedItem();
        
        if ( data is not null )
            return new ServiceReply<CategoryDataDto?>( data );

        ServiceReply<List<CategoryLightDto>?> reply = await TryGetRequest<List<CategoryLightDto>?>( API_PATH );

        if ( !reply.Success || reply.Payload is null )
            return new ServiceReply<CategoryDataDto?>( reply.ErrorType, reply.Message );

        data = new CategoryDataDto( reply.Payload );
        await TrySetCachedItem( data );

        return new ServiceReply<CategoryDataDto?>( data );
    }
}