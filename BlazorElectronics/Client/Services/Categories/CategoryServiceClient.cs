using System.Net.Http.Json;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DataTransferObjects.Categories;

namespace BlazorElectronics.Client.Services.Categories;

public class CategoryServiceClient : ICategoryServiceClient
{
    readonly HttpClient _http;
    Categories_DTO? Categories { get; set; }

    public CategoryServiceClient( HttpClient http )
    {
        _http = http;
    }

    public async Task<ServiceResponse<Categories_DTO?>> GetCategories()
    {
        if ( Categories != null )
            return new ServiceResponse<Categories_DTO?>( Categories, true, "Successfully retrieved categories from local stash." );

        try
        {
            var response = await _http.GetFromJsonAsync<ServiceResponse<Categories_DTO?>>( "api/Category/categories" );

            if ( response == null )
                return new ServiceResponse<Categories_DTO?>( null, false, "Category response is null!" );
            if ( response.Data == null )
                return new ServiceResponse<Categories_DTO?>( null, false, response.Message ??= "Failed to retrieve Categories; no response message!" );

            Categories = response.Data;
            return response;
        }
        catch ( Exception e )
        {
            return new ServiceResponse<Categories_DTO?>( null, false, e.Message );
        }
    }
}