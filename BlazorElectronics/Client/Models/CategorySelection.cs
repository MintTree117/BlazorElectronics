using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Client.Models;

public sealed class CategorySelection
{
    public List<CategorySelectionOption> Options { get; set; } = new();

    public CategorySelection()
    {
        Options = Enum.GetValues( typeof( PrimaryCategory ) )
            .Cast<PrimaryCategory>()
            .Select( category => new CategorySelectionOption
            {
                Category = category,
                IsSelected = false,
            } )
            .ToList();
    }
    
    public void Set( List<PrimaryCategory> modelCategories )
    {
        Options = Enum.GetValues( typeof( PrimaryCategory ) )
            .Cast<PrimaryCategory>()
            .Select( category => new CategorySelectionOption
            {
                Category = category,
                IsSelected = modelCategories.Contains( category ),
            } )
            .ToList();
    }
    public List<PrimaryCategory> GetSelected()
    {
        return Options.Where( c => c.IsSelected )
            .Select( c => c.Category )
            .ToList();
    }
}