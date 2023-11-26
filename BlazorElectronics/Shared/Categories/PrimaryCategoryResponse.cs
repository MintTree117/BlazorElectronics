namespace BlazorElectronics.Shared.Categories;

public sealed class PrimaryCategoryResponse : CategoryResponse
{
    public List<int> ChildCategories { get; init; } = new();
}