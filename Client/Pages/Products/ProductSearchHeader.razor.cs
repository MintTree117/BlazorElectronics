using BlazorElectronics.Client.Shared;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.Products.Search;
using BlazorElectronics.Shared.Vendors;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.Products;

public partial class ProductSearchHeader : RazorView
{
    [Parameter] public EventCallback OnOpenFilters { get; set; }
    [Parameter] public EventCallback<ProductSortType> OnSortChange { get; set; }
    [Parameter] public EventCallback<int> OnRowsChange { get; set; }
    
    List<string> _sortOptions = Enum.GetNames<ProductSortType>().ToList();
    List<int> _rowsPerPageOptions = new() { 4, 8, 16, 32 };
    string _currentSortOption = ProductSortType.Featured.ToString();
    int _selectedSortOption = -1;
    int _currentPage = 1;
    int _totalResults = 0;
    int _rowsPerPage = 8;
    
    Dictionary<string, string> _urls = new();
    
    public void SetBreadcrumbUrls( CategoryFullDto? category, Dictionary<int, CategoryFullDto>? categories )
    {
        Dictionary<string, string> urls = new() { { "Search", Routes.SEARCH } };

        if ( category is null || categories is null )
        {
            _urls = urls;
            StateHasChanged();
            return;
        }

        CategoryFullDto? c = category;
        List<CategoryFullDto> categoryList = new();

        do
        {
            if ( c is null )
                break;

            categoryList.Add( c );

            if ( c.ParentCategoryId is null || !categories.TryGetValue( c.ParentCategoryId.Value, out c ) )
                break;
        } 
        while ( true );

        categoryList.Reverse();

        foreach ( CategoryFullDto cat in categoryList )
        {
            urls.Add( cat.Name, $"{Routes.SEARCH}/{cat.ApiUrl}" );
        }

        _urls = urls;
        StateHasChanged();
    }
    public void OnSearchResults( int page, ProductSearchReplyDto search )
    {
        _totalResults = search.TotalMatches;
        _currentPage = page;
        StateHasChanged();
    }
    public void SetPage( int page )
    {
        _currentPage = page;
        StateHasChanged();
    }

    string GetViewRows()
    {
        int start = 1 + _rowsPerPage * Math.Max( _currentPage - 1, 0 );
        int end = start + _rowsPerPage;

        return $"{start} - {end} of {_totalResults} results";
    }
    
    void OpenFilters()
    {
        OnOpenFilters.InvokeAsync();
    }

    async Task SelectSort( int index )
    {
        if ( index < 0 || index >= _sortOptions.Count )
            return;

        _selectedSortOption = index;
        _currentSortOption = _sortOptions[ index ];

        await OnSortChange.InvokeAsync( ( ProductSortType ) _selectedSortOption );
    }
    async Task SelectRows( int index )
    {
        if ( index < 0 || index >= _rowsPerPageOptions.Count )
            return;
        
        _rowsPerPage = _rowsPerPageOptions[ index ];

        await OnRowsChange.InvokeAsync( _rowsPerPage );
    }
}