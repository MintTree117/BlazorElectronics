using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Categories;

namespace BlazorElectronics.Client.Services.Categories;

public class CategoryServiceClient : ClientService, ICategoryServiceClient
{
    const string API_PATH = "api/category/get";
    CategoriesResponse? _categories;

    public CategoryServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }

    public async Task<ServiceReply<CategoriesResponse?>> GetCategories()
    {
        if ( _categories is not null )
            return new ServiceReply<CategoriesResponse?>( _categories );

        ServiceReply<CategoriesResponse?> reply = await TryGetRequest<CategoriesResponse?>( API_PATH );
        _categories = reply.Data;

        return _categories is not null
            ? new ServiceReply<CategoriesResponse?>( _categories )
            : new ServiceReply<CategoriesResponse?>( reply.ErrorType, reply.Message );
    }
}