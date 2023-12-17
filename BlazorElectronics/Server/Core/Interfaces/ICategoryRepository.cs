using BlazorElectronics.Shared.Categories;

namespace BlazorElectronics.Server.Core.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<CategoryFullDto>?> Get();
    Task<CategoryFullDto?> GetEdit( int categoryId );
    Task<bool> BulkInsert( List<CategoryEditDtoDto> dtos );
    Task<int> Insert( CategoryEditDtoDto dtoDto );
    Task<bool> Update( CategoryEditDtoDto dtoDto );
    Task<bool> Delete( int categoryId );
}