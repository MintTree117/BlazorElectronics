namespace BlazorElectronics.Shared.Outbound.Categories;

public sealed class CategoriesResponse
{
    public CategoriesResponse( List<PrimaryCategoryResponse> primary, List<SecondaryCategoryResponse> secondary, List<TertiaryCategoryResponse> tertiary )
    {
        Primary = primary;
        Secondary = secondary;
        Tertiary = tertiary;
    }
    
    public List<PrimaryCategoryResponse> Primary { get; }
    public List<SecondaryCategoryResponse> Secondary { get; }
    public List<TertiaryCategoryResponse> Tertiary { get; }
}