namespace BlazorElectronics.Shared.Outbound.Categories;

public sealed class SecondaryCategoryResponse
{
    public SecondaryCategoryResponse( short parentId, short id, string name, string url, string imageUrl, HashSet<short> childCategories )
    {
        ParentId = parentId;
        Id = id;
        Name = name;
        Url = url;
        ImageUrl = imageUrl;
        ChildCategories = childCategories;
    }
    
    public short ParentId { get; }
    public short Id { get; }
    public string Name { get; }
    public string Url { get; }
    public string ImageUrl { get; }
    public IReadOnlySet<short> ChildCategories { get; }
}