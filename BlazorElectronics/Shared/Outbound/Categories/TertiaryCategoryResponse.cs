namespace BlazorElectronics.Shared.Outbound.Categories;

public sealed class TertiaryCategoryResponse
{
    public int ParentId { get; set; }
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}