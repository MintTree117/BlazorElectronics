using BlazorElectronics.Shared.Admin.Categories;

namespace BlazorElectronics.Server.Admin.Repositories;

public interface IAdminCategoryRepository
{
    Task<CategoriesViewDto?> GetView();
    Task<CategoryEditDto?> GetEdit( CategoryGetEditDto request );
    Task<CategoryEditDto?> Insert( CategoryAddDto dto );
    Task<bool> Update( CategoryEditDto dto );
    Task<bool> Delete( CategoryRemoveDto dto );
}