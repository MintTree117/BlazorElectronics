using BlazorElectronics.Shared.Categories;

namespace BlazorElectronics.Server.Core.Interfaces;

public interface ICategoryRepository : IDapperRepository
{
    Task<IEnumerable<CategoryFullDto>?> Get();
    Task<CategoryFullDto?> GetEdit( int categoryId );
    Task<bool> BulkInsert( List<CategoryEditDto> dtos );
    Task<int> Insert( CategoryEditDto dto );
    Task<bool> Update( CategoryEditDto dto );
    Task<bool> Delete( int categoryId );
}