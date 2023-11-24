namespace BlazorElectronics.Client.Shared;

public partial class ProductSort : RazorView
{
    string _textSearch = string.Empty;
    
    string _currentSortOption = "Choose an option";
    int _selectedSortOption = -1;
    List<string> _sortOptions = new() { "Lowest Price", "Highest Price", "Best Rating", "Best Selling", "Most Reviews" };

    int _currentPage = 1;
    int _totalPages = 100;

    int _rowsPerPage = 10;
    List<int> _rowsPerPageOptions = new() { 10, 20, 50, 100 };
    
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