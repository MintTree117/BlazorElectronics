namespace BlazorElectronics.Shared.Outbound.Categories;

public sealed class PrimaryCategoryResponse
{
    public PrimaryCategoryResponse( short id, string name, string url, string imageUrl, HashSet<short> childCategories )
    {
        Id = id;
        Name = name;
        Url = url;
        ImageUrl = imageUrl;
        ChildCategories = childCategories;
    }
    
    public short Id { get; }
    public string Name { get; }
    public string Url { get; }
    public string ImageUrl { get; }
    public IReadOnlySet<short> ChildCategories { get; }
}