using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Products.Search;
using BlazorElectronics.Shared.Vendors;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.Products;

public partial class ProductSearchList : RazorView, IDisposable
{
    [Parameter] public ProductSearch Page { get; init; } = default!;
    
    ProductSearchReplyDto? _search;
    Dictionary<int, CategoryFullDto> _categories = new();
    Dictionary<int, VendorDto> _vendors = new();

    public void Dispose()
    {
        Page.OnProductSearch -= OnSearch;
    }
    protected override void OnInitialized()
    {
        base.OnInitialized();
        Page.OnProductSearch += OnSearch;
    }
    void OnSearch( ProductSearchReplyDto? search, Dictionary<int, CategoryFullDto> categories, Dictionary<int, VendorDto> vendors )
    {
        _search = search;
        _categories = categories;
        _vendors = vendors;
        StateHasChanged();
    }
}