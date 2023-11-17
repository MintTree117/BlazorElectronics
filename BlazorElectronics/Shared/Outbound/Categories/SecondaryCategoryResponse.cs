namespace BlazorElectronics.Shared.Outbound.Categories;

public sealed class SecondaryCategoryResponse : CategoryResponse
{
    public short ParentId { get; init; }
    public HashSet<short>? ChildCategories { get; init; }
}