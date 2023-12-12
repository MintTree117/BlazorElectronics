using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Products.Search;
using BlazorElectronics.Shared.Specs;
using BlazorElectronics.Shared.Vendors;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.ProductSearch;

public partial class ProductFilters : RazorView, IDisposable
{
    [Parameter] public ProductSearch Page { get; set; } = default!;
    
    bool InStock { get; set; } = false;
    bool OnSale { get; set; } = false;
    
    int? MinPrice { get; set; } = null;
    int? MaxPrice { get; set; } = null;
    int? MinRating { get; set; } = null;

    List<CategoryModel>? _subCategories = new();
    Dictionary<int, Spec>? _specs = new();
    List<VendorModel>? _vendors = new();

    Dictionary<int, bool> _collapsedSpecs = new();
    Dictionary<(int specId, int specValueId), bool> _selectedSpecs = new();
    Dictionary<int, bool> _selectedVendors = new();

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Page.InitializeFilters += InitializeFilters;
    }
    
    void ToggleCollapse( int specId )
    {
        _collapsedSpecs[ specId ] = !_collapsedSpecs[ specId ];
    }
    void InitializeFilters( List<CategoryModel>? subCategories, Dictionary<int,Spec> specs, List<VendorModel> vendors )
    {
        _subCategories = subCategories;
        _specs = specs;
        _vendors = vendors;
        
        _selectedSpecs = new Dictionary<(int specId, int specValueId), bool>();
        _selectedVendors = new Dictionary<int, bool>();

        foreach ( Spec s in _specs.Values )
        {
            _collapsedSpecs.Add( s.SpecId, true );
            
            for ( int i = 0; i < s.Values.Count; i++ )
            {
                _selectedSpecs.Add( ( s.SpecId, i ), false );
            }
        }

        foreach ( VendorModel v in vendors )
        {
            _selectedVendors.Add( v.VendorId, false );
        }
    }
    void ApplyFilters()
    {
        ProductSearchFilters filters = new()
        {
            InStock = InStock,
            OnSale = OnSale,
            MinPrice = MinPrice,
            MaxPrice = MinPrice,
            MinRating = MinRating
        };

        foreach ( KeyValuePair<(int specId, int specValueId), bool> kvp in _selectedSpecs )
        {
            if ( !kvp.Value )
                continue;

            Dictionary<int, List<int>> d = _specs[ kvp.Key.specId ].IsAvoid
                ? filters.SpecsExlude
                : filters.SpecsInclude;

            if ( !d.TryGetValue( kvp.Key.specId, out List<int>? values ) )
            {
                values = new List<int>();
                d.Add( kvp.Key.specId, values );
            }

            values.Add( kvp.Key.specValueId );
        }

        foreach ( KeyValuePair<int, bool> kvp in _selectedVendors )
        {
            if ( !kvp.Value)
                continue;
            
            filters.Vendors.Add( kvp.Key );
        }
    }

    void SetMinimumRating( int rating )
    {
        
    }
    void OnLookupFilterChanged( int specId, int specValueId, bool isChecked )
    {
        _selectedSpecs[ ( specId, specValueId ) ] = isChecked;
    }
    
    public void Dispose()
    {
        Page.InitializeFilters -= InitializeFilters;
    }
}