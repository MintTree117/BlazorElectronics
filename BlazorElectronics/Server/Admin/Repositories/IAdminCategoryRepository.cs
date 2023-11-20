using BlazorElectronics.Shared.Admin.Categories;

namespace BlazorElectronics.Server.Admin.Repositories;

public interface IAdminCategoryRepository
{
    Task<CategoryViewDto?> GetCategoriesView();
    Task<EditCategoryDto?> GetEditCategory( GetCategoryEditDto request );
    Task<EditCategoryDto?> InsertCategory( AddCategoryDto dto );
    Task<bool> UpdateCategory( EditCategoryDto dto );
    Task<bool> DeleteCategory( RemoveCategoryDto dto );
}