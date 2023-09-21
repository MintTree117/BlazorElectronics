namespace BlazorElectronics.Shared.DataTransferObjects.Categories;

public sealed class Category_DTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsPrimary { get; set; } = false;
}