using System.Net.Http.Json;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Outbound.Categories;

namespace BlazorElectronics.Client.Services.Categories;

public class CategoryServiceClient : ICategoryServiceClient
{
    readonly HttpClient _http;
    CategoriesResponse? Categories { get; set; }

    public CategoryServiceClient( HttpClient http )
    {
        _http = http;
    }

    public async Task<ApiReply<CategoriesResponse?>> GetCategories()
    {
        if ( Categories != null )
            return new ApiReply<CategoriesResponse?>( Categories, true, "Successfully retrieved categories from local stash." );

        try
        {
            var response = await _http.GetFromJsonAsync<ApiReply<CategoriesResponse?>>( "api/Category/categories" );

            if ( response == null )
                return new ApiReply<CategoriesResponse?>( "Category response is null!" );
            
            if ( response.Data == null )
                return new ApiReply<CategoriesResponse?>( response.Message ??= "Failed to retrieve Categories; no response message!" );

            Categories = response.Data;
            return response;
        }
        catch ( Exception e )
        {
            return new ApiReply<CategoriesResponse?>( e.Message );
        }
    }
}