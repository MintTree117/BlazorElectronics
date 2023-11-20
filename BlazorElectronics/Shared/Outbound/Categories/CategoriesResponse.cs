namespace BlazorElectronics.Shared.Outbound.Categories;

public sealed class CategoriesResponse
{
    public CategoriesResponse()
    {
        
    }
    
    public CategoriesResponse( List<PrimaryCategoryResponse> primary, List<SecondaryCategoryResponse> secondary, List<TertiaryCategoryResponse> tertiary )
    {
        Primary = primary;
        Secondary = secondary;
        Tertiary = tertiary;
    }

    public List<PrimaryCategoryResponse> Primary { get; init; } = new();
    public List<SecondaryCategoryResponse> Secondary { get; init; } = new();
    public List<TertiaryCategoryResponse> Tertiary { get; init; } = new();
}