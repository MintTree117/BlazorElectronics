using BlazorElectronics.Shared.Categories;

namespace BlazorElectronics.Client.Models;

public sealed class CategorySelection
{
    public List<CategorySelectionOption> Options { get; set; } = new();

    public CategorySelection( IEnumerable<CategoryView> categories )
    {
        Options = categories
            .Select( category => new CategorySelectionOption
            {
                Id = category.Id,
                Name = category.Name,
                IsSelected = false,
            } )
            .ToList();
    }
    
    public void Set( List<int> modelCategories )
    {
        foreach ( CategorySelectionOption option in Options )
        {
            option.IsSelected = modelCategories.Contains( option.Id );
        }
    }
    public List<int> GetSelected()
    {
        return Options.Where( c => c.IsSelected )
            .Select( c => c.Id )
            .ToList();
    }
}