using BlazorElectronics.Shared.DataTransferObjects.Categories;

namespace BlazorElectronics.Server.Services.Categories;

public interface ICategoryService
{
    Task<ServiceResponse<Categories_DTO?>> GetCategories();
    Task<ServiceResponse<int>> CategoryIdFromUrl( string categoryUrl );
}