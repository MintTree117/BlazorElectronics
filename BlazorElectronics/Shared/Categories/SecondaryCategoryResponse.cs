namespace BlazorElectronics.Shared.Categories;

public sealed class SecondaryCategoryResponse : CategoryResponse
{
    public int ParentId { get; init; }
    public List<int> ChildCategories { get; init; } = new();
}