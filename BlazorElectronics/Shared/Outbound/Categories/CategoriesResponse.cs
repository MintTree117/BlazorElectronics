namespace BlazorElectronics.Shared.Outbound.Categories;

public sealed class CategoriesResponse
{
    public CategoriesResponse( IReadOnlyList<PrimaryCategoryResponse> primary, IReadOnlyList<SecondaryCategoryResponse> secondary, IReadOnlyList<TertiaryCategoryResponse> tertiary )
    {
        Primary = primary;
        Secondary = secondary;
        Tertiary = tertiary;
    }
    
    public IReadOnlyList<PrimaryCategoryResponse> Primary { get; }
    public IReadOnlyList<SecondaryCategoryResponse> Secondary { get; }
    public IReadOnlyList<TertiaryCategoryResponse> Tertiary { get; }
}