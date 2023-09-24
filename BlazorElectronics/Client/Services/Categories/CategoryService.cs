using System.Net.Http.Json;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DataTransferObjects.Categories;

namespace BlazorElectronics.Client.Services.Categories;

public class CategoryService : ICategoryService
{
    public string ControllerMessage { get; set; } = string.Empty;
    public List<Category_DTO>? Categories { get; set; }

    readonly HttpClient _http;

    public CategoryService( HttpClient http )
    {
        _http = http;
    }

    public async Task GetCategories()
    {
        var response = await _http.GetFromJsonAsync<ControllerResponse<Categories_DTO>>( "api/Category/categories" );
        if ( response is { Data: not null } )
            Categories = response.Data.Categories;
    }
}