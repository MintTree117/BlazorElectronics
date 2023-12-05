namespace BlazorElectronics.Client.Models;

public sealed class CategorySelectionOption
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsSelected { get; set; }
}