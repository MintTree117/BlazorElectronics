namespace BlazorElectronics.Shared.Categories;

public sealed class CategoriesResponse
{
    public CategoriesResponse()
    {
        
    }
    
    public CategoriesResponse( IReadOnlyList<CategoryResponse> primary, IReadOnlyList<CategoryResponse> secondary, IReadOnlyList<CategoryResponse> tertiary )
    {
        Primary = primary.ToList();
        Secondary = secondary.ToList();
        Tertiary = tertiary.ToList();
    }

    public List<CategoryResponse> Primary { get; init; } = new();
    public List<CategoryResponse> Secondary { get; init; } = new();
    public List<CategoryResponse> Tertiary { get; init; } = new();
}