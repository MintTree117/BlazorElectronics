using BlazorElectronics.Server.Models.Categories;
using BlazorElectronics.Shared.Categories;

namespace BlazorElectronics.Server.Repositories.Categories;

public interface ICategoryRepository
{
    Task<CategoriesModel?> Get();
    Task<CategoriesViewDto?> GetView();
    Task<CategoryEditDto?> GetEdit( CategoryGetEditDto request );
    Task<CategoryEditDto?> Insert( CategoryAddDto dto );
    Task<bool> Update( CategoryEditDto dto );
    Task<bool> Delete( CategoryRemoveDto dto );
}