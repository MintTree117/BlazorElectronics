namespace BlazorElectronics.Shared.Outbound.Categories;

public sealed class PrimaryCategoryResponse : CategoryResponse
{
    public HashSet<short>? ChildCategories { get; init; }
}