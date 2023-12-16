using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.Products.Search;
using BlazorElectronics.Shared.Vendors;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.Products;

public partial class ProductSearchHeader : RazorView, IDisposable
{
    [Parameter] public Products.ProductSearch Page { get; set; } = default!;
    
    string _currentSortOption = ProductSortType.Featured.ToString();
    List<string> _sortOptions = Enum.GetNames<ProductSortType>().ToList();
    int _selectedSortOption = -1;

    int _currentPage = 1;
    int _totalPages = 100;
    int _totalResults = 0;

    int _rowsPerPage = 10;
    List<int> _rowsPerPageOptions = new() { 10, 20, 50, 100 };

    Dictionary<string, string> _urls = new();

    public void Dispose()
    {
        Page.InitializeHeader -= SetBreadcrumbUrls;
        Page.OnProductSearch -= OnSearchResults;
    }
    protected override void OnInitialized()
    {
        Page.InitializeHeader += SetBreadcrumbUrls;
        Page.OnProductSearch += OnSearchResults;
    }
    void SetBreadcrumbUrls( Dictionary<string,string> urls )
    {
        _urls = urls;
        StateHasChanged();
    }
    void OnSearchResults( ProductSearchResponse search, Dictionary<int, CategoryModel> categories, Dictionary<int, VendorModel> vendors )
    {
        _totalResults = search.TotalMatches;
        StateHasChanged();
    }

    async Task SelectSort( int index )
    {
        if ( index < 0 || index >= _sortOptions.Count )
            return;

        _selectedSortOption = index;
        _currentSortOption = _sortOptions[ index ];

        await Page.ApplySort( ( ProductSortType ) _selectedSortOption );
    }
    async Task SelectRows( int index )
    {
        if ( index < 0 || index >= _rowsPerPageOptions.Count )
            return;
        
        _rowsPerPage = _rowsPerPageOptions[ index ];

        await Page.ApplyRows( _rowsPerPage );
    }
}