namespace BlazorElectronics.Shared.Categories;

public sealed class CategoriesResponse
{
    public CategoriesResponse()
    {
        
    }
    
    public CategoriesResponse( IReadOnlyList<PrimaryCategoryResponse> primary, IReadOnlyList<SecondaryCategoryResponse> secondary, IReadOnlyList<TertiaryCategoryResponse> tertiary )
    {
        Primary = primary.ToList();
        Secondary = secondary.ToList();
        Tertiary = tertiary.ToList();
    }

    public List<PrimaryCategoryResponse> Primary { get; init; } = new();
    public List<SecondaryCategoryResponse> Secondary { get; init; } = new();
    public List<TertiaryCategoryResponse> Tertiary { get; init; } = new();
}