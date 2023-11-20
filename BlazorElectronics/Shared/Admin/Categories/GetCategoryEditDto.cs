namespace BlazorElectronics.Shared.Admin.Categories;

public sealed class GetCategoryEditDto
{
    public GetCategoryEditDto()
    {
        
    }
    public GetCategoryEditDto( int id, int tier )
    {
        CategoryId = id;
        CategoryTier = tier;
    }
    
    public int CategoryId { get; init; }
    public int CategoryTier { get; init; }
}