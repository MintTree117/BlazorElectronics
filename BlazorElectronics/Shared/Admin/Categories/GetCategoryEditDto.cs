namespace BlazorElectronics.Shared.Admin.Categories;

public sealed class GetCategoryEditDto
{
    public GetCategoryEditDto()
    {
        
    }
    public GetCategoryEditDto( CategoryType type, int id )
    {
        CategoryType = type;
        CategoryId = id;
    }

    public CategoryType CategoryType { get; init; }
    public int CategoryId { get; init; }
}