using System.Net.Http.Json;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Outbound.Categories;

namespace BlazorElectronics.Client.Services.Categories;

public class CategoryServiceClient : ICategoryServiceClient
{
    readonly HttpClient _http;
    CategoriesResponse? Categories;

    public CategoryServiceClient( HttpClient http )
    {
        _http = http;
    }

    public async Task<ApiReply<CategoriesResponse?>> GetCategories()
    {
        if ( Categories is not null )
            return new ApiReply<CategoriesResponse?>( Categories );

        ApiReply<CategoriesResponse?>? reply;
        
        try
        {
            reply = await _http.GetFromJsonAsync<ApiReply<CategoriesResponse?>?>( "api/Category/categories" );
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