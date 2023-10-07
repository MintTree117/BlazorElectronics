using System.Net.Http.Json;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DataTransferObjects.Categories;

namespace BlazorElectronics.Client.Services.Categories;

public class CategoryServiceClient : ICategoryServiceClient
{
    public string ControllerMessage { get; set; } = string.Empty;
    public List<Category_DTO>? Categories { get; set; }

    readonly HttpClient _http;

    public CategoryServiceClient( HttpClient http )
    {
        _http = http;
    }

    public async Task<Categories_DTO?> GetCategories()
    {
        var response = await _http.GetFromJsonAsync<ServiceResponse<Categories_DTO?>>( "api/Category/categories" );
        return response?.Data;
    }
}