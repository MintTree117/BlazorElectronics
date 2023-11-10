namespace BlazorElectronics.Shared.Outbound.Categories;

public sealed class TertiaryCategoryResponse
{
    public TertiaryCategoryResponse( short parentId, short id, string name, string url, string imageUrl )
    {
        ParentId = parentId;
        Id = id;
        Name = name;
        Url = url;
        ImageUrl = imageUrl;
    }
    
    public short ParentId { get; }
    public short Id { get; }
    public string Name { get; }
    public string Url { get; }
    public string ImageUrl { get; }
}