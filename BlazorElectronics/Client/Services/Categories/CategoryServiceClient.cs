using System.Net.Http.Json;
using Blazored.LocalStorage;
using BlazorElectronics.Client.Services.Users;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Outbound.Categories;

namespace BlazorElectronics.Client.Services.Categories;

public class CategoryServiceClient : ClientService, ICategoryServiceClient
{
    CategoriesResponse? Categories;

    public CategoryServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }

    public async Task<ApiReply<CategoriesResponse?>> GetCategories()
    {
        if ( Categories is not null )
            return new ApiReply<CategoriesResponse?>( Categories );

        ApiReply<CategoriesResponse?>? reply;
        
        try
        {
            reply = await Http.GetFromJsonAsync<ApiReply<CategoriesResponse?>?>( "api/Category/categories" );
        }
        catch ( Exception e )
        {
            return new ApiReply<CategoriesResponse?>( e.Message );
        }

        if ( reply is null || !reply.Success || reply.Data is null )
            return new ApiReply<CategoriesResponse?>( reply?.Message );

        Categories = reply.Data;

        return new ApiReply<CategoriesResponse?>( Categories );
    }
}