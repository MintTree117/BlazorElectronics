using BlazorElectronics.Shared.DataTransferObjects.Categories;

namespace BlazorElectronics.Server.Services.Categories;

public interface ICategoryService
{
    Task<ServiceResponse<CategoryLists_DTO?>> GetCategories();
    Task<ServiceResponse<string?>> GetCategoryName( string categoryUrl );
}