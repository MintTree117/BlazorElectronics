namespace BlazorElectronics.Shared.Outbound.Categories;

public sealed class CategoriesResponse
{
    public Dictionary<int, PrimaryCategoryResponse> PrimaryCategories { get; set; } = new();
    public Dictionary<int, SecondaryCategoryResponse> SecondaryCategories { get; set; } = new();
    public Dictionary<int, TertiaryCategoryResponse> TertiaryCategories { get; set; } = new();
}