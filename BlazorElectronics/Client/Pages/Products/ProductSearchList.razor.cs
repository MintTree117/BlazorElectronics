using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Products.Search;
using BlazorElectronics.Shared.Vendors;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.Products;

public partial class ProductSearchList : RazorView, IDisposable
{
    [Parameter] public ProductSearch Page { get; init; } = default!;
    
    ProductSearchResponse? _search;
    Dictionary<int, CategoryModel> _categories = new();
    Dictionary<int, VendorModel> _vendors = new();

    public void Dispose()
    {
        Page.OnProductSearch -= OnSearch;
    }
    protected override void OnInitialized()
    {
        base.OnInitialized();
        Page.OnProductSearch += OnSearch;
    }
    void OnSearch( ProductSearchResponse? search, Dictionary<int, CategoryModel> categories, Dictionary<int, VendorModel> vendors )
    {
        _search = search;
        _categories = categories;
        _vendors = vendors;
        StateHasChanged();
    }
}