using BlazorElectronics.Server.Models.Categories;
using BlazorElectronics.Shared.Categories;

namespace BlazorElectronics.Server.Repositories.Categories;

public interface ICategoryRepository
{
    Task<IEnumerable<CategoryModel>?> Get();
    Task<CategoryModel?> GetEdit( int categoryId );
    Task<CategoryModel?> Insert( CategoryEditDto dto );
    Task<bool> Update( CategoryEditDto dto );
    Task<bool> Delete( int categoryId );
}