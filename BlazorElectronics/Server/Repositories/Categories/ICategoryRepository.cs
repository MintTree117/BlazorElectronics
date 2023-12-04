using BlazorElectronics.Server.Models.Categories;
using BlazorElectronics.Shared.Categories;

namespace BlazorElectronics.Server.Repositories.Categories;

public interface ICategoryRepository
{
    Task<IEnumerable<CategoryModel>?> Get();
    Task<CategoryModel?> GetEdit( int categoryId );
    Task<int> Insert( CategoryEdit dto );
    Task<bool> Update( CategoryEdit dto );
    Task<bool> Delete( int categoryId );
}