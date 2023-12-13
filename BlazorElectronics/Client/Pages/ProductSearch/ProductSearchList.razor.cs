using BlazorElectronics.Shared.Products.Search;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.ProductSearch;

public partial class ProductSearchList : RazorView, IDisposable
{
    [Parameter] public ProductSearch Page { get; init; } = default!;

    ProductSearchResponse? _search;

    public void Dispose()
    {
        Page.OnProductSearch -= OnSearch;
    }
    protected override void OnInitialized()
    {
        base.OnInitialized();
        Page.OnProductSearch += OnSearch;
    }
    void OnSearch( ProductSearchResponse? search )
    {
        _search = search;
        StateHasChanged();
    }
}