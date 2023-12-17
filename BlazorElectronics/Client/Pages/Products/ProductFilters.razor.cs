using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Products.Search;
using BlazorElectronics.Shared.Specs;
using BlazorElectronics.Shared.Vendors;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.Products;

public partial class ProductFilters : RazorView, IDisposable
{
    [Parameter] public ProductSearch Page { get; set; } = default!;

    const string SECTION_CATEGORY = "Categories";
    const string SECTION_TRENDS = "Trends";
    const string SECTION_PRICE = "Pricing";
    const string SECTION_RATING = "Rating";
    const string SECTION_VENDORS = "Vendors";

    string _showFiltersCss = string.Empty;
    string _searchText = string.Empty;
    
    bool InStock { get; set; } = false;
    bool Featured { get; set; } = false;
    bool OnSale { get; set; } = false;

    decimal? MinPrice { get; set; } = null;
    decimal? MaxPrice { get; set; } = null;
    int? MinRating { get; set; } = null;
    
    List<CategoryFullDto>? _subCategories = new();
    Dictionary<int, LookupSpec> _specs = new();
    List<VendorDto> _vendors = new();
    
    Dictionary<string, bool> _collapsedSections = new();
    Dictionary<(int specId, int specValueId), bool> _selectedSpecs = new();
    Dictionary<int, bool> _selectedVendors = new();

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Page.InitializeFilters += InitializeFilters;
        Page.OnOpenFilters += ShowFilters;
        _collapsedSections.Add( SECTION_CATEGORY, false );
        _collapsedSections.Add( SECTION_TRENDS, false );
        _collapsedSections.Add( SECTION_PRICE, false );
        _collapsedSections.Add( SECTION_RATING, false );
        _collapsedSections.Add( SECTION_VENDORS, true );
    }

    void ShowFilters()
    {
        _showFiltersCss = "d-flex";
        StateHasChanged();
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
    void InitializeFilters( string? searchText, List<CategoryFullDto>? subCategories, Dictionary<int,LookupSpec> specs, List<VendorDto> vendors )
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
            {
                _selectedSpecs.Add( ( s.SpecId, i ), false );
            }
        }

        foreach ( VendorDto v in vendors )
        {
            _selectedVendors.Add( v.VendorId, false );
        }
    }
    async Task ApplyFilters()
    {
        ProductFiltersDto filtersDto = new()
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
                ? filtersDto.SpecsExlude
                : filtersDto.SpecsInclude;

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
            
            filtersDto.Vendors.Add( kvp.Key );
        }

        await Page.ApplyFilters( filtersDto );
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
        Page.OnOpenFilters -= ShowFilters;
    }
}