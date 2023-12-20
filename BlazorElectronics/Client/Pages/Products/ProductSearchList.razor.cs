using BlazorElectronics.Client.Services.Cart;
using BlazorElectronics.Shared;
using BlazorElectronics.Client.Shared;
using BlazorElectronics.Shared.Cart;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Products.Search;
using BlazorElectronics.Shared.Vendors;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.Products;

public partial class ProductSearchList : RazorView
{
    [Inject] public ICartServiceClient CartService { get; set; } = default!;
    [Parameter] public EventCallback<int> OnPageChanged { get; set; }
    Pagination _pagination = default!;
    
    ProductSearchReplyDto? _search;
    Dictionary<int, CategoryFullDto> _categories = new();
    Dictionary<int, VendorDto> _vendors = new();

    public void SetPage( int page )
    {
        _pagination.SetCurrentPage( page );
    }
    public void OnSearch( int rows, ProductSearchReplyDto? search, Dictionary<int, CategoryFullDto> categories, Dictionary<int, VendorDto> vendors )
    {
        _search = search;
        _categories = categories;
        _vendors = vendors;
        _pagination.UpdateTotalPages( rows, search?.TotalMatches ?? 0 );
        StateHasChanged();
    }

    async Task AddToCart( ProductSummaryDto p )
    {
        await CartService.AddOrUpdateItem( new CartProductDto( p ) );
    }
}