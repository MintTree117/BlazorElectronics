using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Categories;

namespace BlazorElectronics.Client.Services.Categories;

public class CategoryServiceClient : ClientService, ICategoryServiceClient
{
    const string API_PATH = "api/category/get";
    CategoryData? _categories;

    public CategoryServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }

    public async Task<ServiceReply<CategoryData?>> GetCategories()
    {
        if ( _categories is not null )
            return new ServiceReply<CategoryData?>( _categories );

        ServiceReply<List<CategoryResponse>?> reply = await TryGetRequest<List<CategoryResponse>?>( API_PATH );

        if ( !reply.Success || reply.Data is null )
            return new ServiceReply<CategoryData?>( reply.ErrorType, reply.Message );

        _categories = new CategoryData( reply.Data );

        return _categories is not null
            ? new ServiceReply<CategoryData?>( _categories )
            : new ServiceReply<CategoryData?>( reply.ErrorType, reply.Message );
    }
}