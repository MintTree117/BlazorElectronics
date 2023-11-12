using BlazorElectronics.Shared.Admin.Categories;

namespace BlazorElectronics.Server.Admin.Repositories;

public interface IAdminCategoryRepository
{
    Task<bool> AddCategory( AddUpdateCategoryDto dto );
    Task<bool> UpdateCategory( AddUpdateCategoryDto dto );
    Task<bool> DeleteCategory( DeleteCategoryDto dto );
}