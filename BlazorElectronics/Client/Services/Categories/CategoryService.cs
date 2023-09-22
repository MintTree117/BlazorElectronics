using System.Net.Http.Json;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DataTransferObjects.Categories;

namespace BlazorElectronics.Client.Services.Categories;

public class CategoryService : ICategoryService
{
    public string ControllerMessage { get; set; } = string.Empty;
    public List<Category_DTO>? PrimaryCategories { get; set; }
    public List<CategorySub_DTO>? SubCategories { get; set; }

    readonly HttpClient _http;

    public CategoryService( HttpClient http )
    {
        _http = http;
    }

    public async Task GetCategories()
    {
        var response = await _http.GetFromJsonAsync<ControllerResponse<CategoryLists_DTO>>( "api/Category/categories" );

        if ( response is { Data: not null } ) {
            PrimaryCategories = response.Data.PrimaryCategories;
            SubCategories = response.Data.SubCategories;
        }
    }
}