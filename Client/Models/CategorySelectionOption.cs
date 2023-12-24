namespace BlazorElectronics.Client.Models;

public sealed class CategorySelectionOption
{
    public CategorySelectionOption( int id, string name, bool isSelected = false )
    {
        Id = id;
        Name = name;
    }
    
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsSelected { get; set; }
}