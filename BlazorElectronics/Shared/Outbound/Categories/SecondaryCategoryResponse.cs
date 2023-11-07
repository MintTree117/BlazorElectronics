namespace BlazorElectronics.Shared.Outbound.Categories;

public sealed class SecondaryCategoryResponse
{
    public short ParentId { get; set; }
    public short Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public HashSet<short> ChildCategories { get; set; } = new();
}