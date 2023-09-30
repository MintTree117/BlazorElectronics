using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DataTransferObjects.Categories;

namespace BlazorElectronics.Server.Services.Categories;

public interface ICategoryService
{
    Task<DtoResponse<Categories_DTO?>> GetCategories();
    Task<DtoResponse<int>> GetCategoryIdFromUrl( string url );
}