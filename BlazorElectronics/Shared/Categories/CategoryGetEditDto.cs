using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Shared.Categories;

public sealed class CategoryGetEditDto
{
    public CategoryGetEditDto()
    {
        
    }
    public CategoryGetEditDto( CategoryType type, int id )
    {
        CategoryType = type;
        CategoryId = id;
    }

    public CategoryType CategoryType { get; init; }
    public int CategoryId { get; init; }
}