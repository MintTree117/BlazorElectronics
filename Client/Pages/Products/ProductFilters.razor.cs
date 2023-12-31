using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Products.Search;
using BlazorElectronics.Shared.Specs;
using BlazorElectronics.Shared.Vendors;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.Products;

public partial class ProductFilters : RazorView
{
    [Parameter] public EventCallback<ProductFiltersDto> OnApplyFilters { get; set; }

    const string SECTION_CATEGORY = "Categories";
    const string SECTION_TRENDS = "Trends";
    const string SECTION_PRICE = "Pricing";
    const string SECTION_RATING = "Rating";
    const string SECTION_VENDORS = "Vendors";

    string _showFiltersCss = string.Empty;
    string _searchText = string.Empty;
    
    ProductFiltersDto _filters = new();

    IReadOnlyList<CategoryFullDto>? _subCategories = new List<CategoryFullDto>();
    IReadOnlyDictionary<int, LookupSpec> _specs = new Dictionary<int, LookupSpec>();
    IReadOnlyList<VendorDto> _vendors = new List<VendorDto>();

    int? _selectedRating = null;
    Dictionary<string, bool> _collapsedSections = new();
    Dictionary<(int specId, int specValueId), bool> _selectedSpecs = new();
    Dictionary<int, bool> _selectedVendors = new();

    public void InitializeFilters( string? searchText, IReadOnlyList<CategoryFullDto>? subCategories, IReadOnlyDictionary<int, LookupSpec> specs, IReadOnlyList<VendorDto> vendors )
    {
        _searchText = searchText ?? string.Empty;
        _subCategories = subCategories;
        _specs = specs;
        _vendors = vendors;

        _selectedSpecs = new Dictionary<(int specId, int specValueId), bool>();
        _selectedVendors = new Dictionary<int, bool>();

        foreach ( LookupSpec s in _specs.Values )
        {
            _collapsedSections.Add( s.SpecName, true );

            for ( int i = 0; i < s.Values.Count; i++ )
                _selectedSpecs.Add( ( s.SpecId, i ), false );
        }

        foreach ( VendorDto v in vendors )
            _selectedVendors.Add( v.VendorId, false );
        
        StateHasChanged();
    }
    public void ShowFilters()
    {
        _showFiltersCss = "d-flex";
        StateHasChanged();
    }
    
    protected override void OnInitialized()
    {
        base.OnInitialized();
        _collapsedSections.Add( SECTION_CATEGORY, false );
        _collapsedSections.Add( SECTION_TRENDS, false );
        _collapsedSections.Add( SECTION_PRICE, false );
        _collapsedSections.Add( SECTION_RATING, false );
        _collapsedSections.Add( SECTION_VENDORS, true );
    }
    
    void HideFilters()
    {
        _showFiltersCss = "d-none";
        StateHasChanged();
    }
    void ToggleSectionCollapse( string sectionName )
    {
        _collapsedSections[ sectionName ] = !_collapsedSections[ sectionName ];
    }
    string GetCollapseIcon( string sectionName )
    {
        return _collapsedSections[ sectionName ] ? "oi-plus" : "oi-minus";
    }
    string GetCollapseDisplay( string sectionName )
    {
        return _collapsedSections[ sectionName ] ? "collapse" : "collapse-show";
    }
    async Task ApplyFilters()
    {
        foreach ( KeyValuePair<(int specId, int specValueId), bool> kvp in _selectedSpecs )
        {
            if ( !kvp.Value )
                continue;

            Dictionary<int, List<int>> d = _specs[ kvp.Key.specId ].IsAvoid
                ? _filters.SpecsExlude
                : _filters.SpecsInclude;

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
            
            _filters.Vendors.Add( kvp.Key );
        }

        await OnApplyFilters.InvokeAsync( _filters );
    }
    void ClearFilters()
    {
        _filters = new ProductFiltersDto();
        _selectedRating = null;
        
        foreach ( (int specId, int specValueId) key in _selectedSpecs.Keys )
        {
            _selectedSpecs[ key ] = false;
        }
        foreach ( var key in _selectedVendors.Keys )
        {
            _selectedVendors[ key ] = false;
        }
        
        StateHasChanged();
    }

    void SetMinimumRating( int rating )
    {
        _filters.MinRating = rating;
        _selectedRating = rating;
        StateHasChanged();
    }
    void OnLookupFilterChanged( int specId, int specValueId, bool isChecked )
    {
        _selectedSpecs[ ( specId, specValueId ) ] = isChecked;
    }
}