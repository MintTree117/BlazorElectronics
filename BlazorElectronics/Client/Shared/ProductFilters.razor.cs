using BlazorElectronics.Shared.Outbound.Specs;

namespace BlazorElectronics.Client.Shared;

public partial class ProductFilters : RazorView
{
    bool InStock { get; set; } = false;
    bool MustHaveSale { get; set; } = false;

    int? MinPrice { get; set; } = null;
    int? MaxPrice { get; set; } = null;
    int? MinRating { get; set; } = null;

    SpecFiltersResponse _specFilters = new();

    Dictionary<(int FilterId, string Value), bool> selectedIntFilters = new();
    Dictionary<(int FilterId, string Value), bool> selectedStringFilters = new();
    Dictionary<int, bool> selectedBoolFilters = new();
    Dictionary<(int FilterId, string Value), bool> selectedMultiFilters = new();

    void ApplyFilters()
    {
        
    }

    void SetMinimumRating( int rating )
    {
        
    }

    void InitializeSpecFilters()
    {
        foreach ( SpecFilterTableResponse filter in _specFilters.IntFilters )
        {
            foreach ( string value in filter.Values )
            {
                selectedIntFilters[ ( filter.Id, value ) ] = false;
            }
        }
        foreach ( SpecFilterTableResponse filter in _specFilters.StringFilters )
        {
            foreach ( string value in filter.Values )
            {
                selectedStringFilters[ ( filter.Id, value ) ] = false;
            }
        }
        for ( int i = 0; i < _specFilters.BoolFilters.Count; i++ )
        {
            selectedBoolFilters[ i ] = false;
        }
        foreach ( SpecFilterTableResponse filter in _specFilters.MultiFilters )
        {
            foreach ( string value in filter.Values )
            {
                selectedMultiFilters[ ( filter.Id, value ) ] = false;
            }
        }
    }
    
    void OnIntFilterChanged( int filterId, string value, bool isChecked )
    {
        selectedIntFilters[ ( filterId, value ) ] = isChecked;
    }
    void OnStringFilterChanged( int filterId, string value, bool isChecked )
    {
        selectedStringFilters[ ( filterId, value ) ] = isChecked;
    }
    void OnBoolFilterChanged( int filterId, bool isChecked )
    {
        selectedBoolFilters[ filterId ] = isChecked;
    }
    void OnMultiFilterChanged( int filterId, string value, bool isChecked )
    {
        selectedMultiFilters[ ( filterId, value ) ] = isChecked;
    }
}