using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Products.Search;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.ProductSearch;

public partial class ProductSearchList : RazorView, IDisposable
{
    [Parameter] public ProductSearch Page { get; init; } = default!;
    
    ProductSearchResponse? _search;
    Dictionary<int, CategoryModel> _categories = new();

    public void Dispose()
    {
        Page.OnProductSearch -= OnSearch;
    }
    protected override void OnInitialized()
    {
        base.OnInitialized();
        Page.OnProductSearch += OnSearch;
    }
    void OnSearch( ProductSearchResponse? search, Dictionary<int, CategoryModel> categories )
    {
        _search = search;
        _categories = categories;
        StateHasChanged();
    }
}