namespace BlazorElectronics.Shared.Outbound.Categories;

public sealed class CategoriesResponse
{
    public Dictionary<short, PrimaryCategoryResponse> PrimaryCategories { get; set; } = new();
    public Dictionary<short, SecondaryCategoryResponse> SecondaryCategories { get; set; } = new();
    public Dictionary<short, TertiaryCategoryResponse> TertiaryCategories { get; set; } = new();
}