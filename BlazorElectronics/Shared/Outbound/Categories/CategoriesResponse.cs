namespace BlazorElectronics.Shared.Outbound.Categories;

public sealed class CategoriesResponse
{
    public CategoriesResponse( Dictionary<short, PrimaryCategoryResponse> primary, Dictionary<short, SecondaryCategoryResponse> secondary, Dictionary<short, TertiaryCategoryResponse> tertiary )
    {
        PrimaryCategories = primary;
        SecondaryCategories = secondary;
        TertiaryCategories = tertiary;
    }
    
    public IReadOnlyDictionary<short, PrimaryCategoryResponse> PrimaryCategories { get; init; }
    public IReadOnlyDictionary<short, SecondaryCategoryResponse> SecondaryCategories { get; init; }
    public IReadOnlyDictionary<short, TertiaryCategoryResponse> TertiaryCategories { get; init; }
}