using BlazorElectronics.Shared;
using BlazorElectronics.Shared.SpecLookups;

namespace BlazorElectronics.Client.Shared;

public partial class ProductFilters : RazorView
{
    bool InStock { get; set; } = false;
    bool MustHaveSale { get; set; } = false;

    int? MinPrice { get; set; } = null;
    int? MaxPrice { get; set; } = null;
    int? MinRating { get; set; } = null;

    List<SpecLookup> _specFilters = new();
    Dictionary<(int specId, int specValueId), bool> _selectedFilters = new();

    void ApplyFilters()
    {
        
    }

    void SetMinimumRating( int rating )
    {
        
    }
    void InitializeSpecFilters()
    {

    }
    void OnLookupFilterChanged( int specId, int specValueId, bool isChecked )
    {
        _selectedFilters[ ( specId, specValueId ) ] = isChecked;
    }
}