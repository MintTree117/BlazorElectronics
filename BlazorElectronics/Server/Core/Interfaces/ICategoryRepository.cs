using BlazorElectronics.Shared.Categories;

namespace BlazorElectronics.Server.Core.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<CategoryModel>?> Get();
    Task<CategoryModel?> GetEdit( int categoryId );
    Task<bool> BulkInsert( List<CategoryEdit> dtos );
    Task<int> Insert( CategoryEdit dto );
    Task<bool> Update( CategoryEdit dto );
    Task<bool> Delete( int categoryId );
}