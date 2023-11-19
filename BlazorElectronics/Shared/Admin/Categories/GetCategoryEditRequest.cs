namespace BlazorElectronics.Shared.Admin.Categories;

public sealed class GetCategoryEditRequest
{
    public GetCategoryEditRequest()
    {
        
    }
    public GetCategoryEditRequest( int id, int tier )
    {
        CategoryId = id;
        CategoryTier = tier;
    }
    
    public int CategoryId { get; init; }
    public int CategoryTier { get; init; }
}