using BlazorElectronics.Shared.Admin.Categories;

namespace BlazorElectronics.Server.Admin.Repositories;

public interface IAdminCategoryRepository
{
    Task<bool> AddCategory( AddCategoryDto dto );
    Task<bool> UpdateCategory( UpdateCategoryDto dto );
    Task<bool> DeleteCategory( DeleteCategoryDto dto );
}