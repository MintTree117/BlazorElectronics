using BlazorElectronics.Shared.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.ProductSearch;

public partial class ProductSearchHeader : RazorView, IDisposable
{
    [Parameter] public ProductSearch Page { get; set; } = default!;
    
    string _currentSortOption = ProductSortType.Featured.ToString();
    List<string> _sortOptions = Enum.GetNames<ProductSortType>().ToList();
    int _selectedSortOption = -1;

    int _currentPage = 1;
    int _totalPages = 100;

    int _rowsPerPage = 10;
    List<int> _rowsPerPageOptions = new() { 10, 20, 50, 100 };

    Dictionary<string, string> _urls = new();

    public void Dispose()
    {
        Page.InitializeHeader -= SetBreadcrumbUrls;
    }
    protected override void OnInitialized()
    {
        Page.InitializeHeader += SetBreadcrumbUrls;
    }
    void SetBreadcrumbUrls( Dictionary<string,string> urls )
    {
        _urls = urls;
        StateHasChanged();
    }

    void SelectOption( int index )
    {
        if ( index < 0 || index >= _sortOptions.Count )
            return;

        _selectedSortOption = index;
        _currentSortOption = _sortOptions[ index ];
    }
    void PreviousPage()
    {
        if ( _currentPage > 1 )
        {
            _currentPage--;
        }
    }
    void NextPage()
    {
        if ( _currentPage < _totalPages )
        {
            _currentPage++;
        }
    }
    void SelectRowsPerPage( int num )
    {
        _rowsPerPage = num;
    }
}