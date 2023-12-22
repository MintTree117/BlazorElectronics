using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Shared.Products;
using BlazorElectronics.Shared.Products.Search;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class ProductsController : _Controller
{
    readonly IProductService _productService;

    public ProductsController( ILogger<_Controller> logger, IProductService productService )
        : base( logger )
    {
        _productService = productService;
    }
    
    [HttpGet( "search-query" )]
    public async Task<ActionResult<ProductSummaryDto?>> SearchQuery()
    {
        ProductSearchRequestDto r = new()
        {
            CategoryId = 1,
            SearchText = "book",
            Filters = new ProductFiltersDto
            {
                SpecsInclude = new Dictionary<int, List<int>>() { { 1, new List<int>(){ 1 } } }
            }
        };
        ServiceReply<string?> reply = await _productService.GetProductSearchQueryString( r );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "search" )]
    public async Task<ActionResult<ProductSummaryDto?>> SearchProducts( [FromBody] ProductSearchRequestDto requestDto )
    {
        ServiceReply<ProductSearchReplyDto?> reply = await _productService.GetProductSearch( requestDto );
        return GetReturnFromReply( reply );
    }
    [HttpGet( "suggestions" )]
    public async Task<ActionResult<List<string>?>> SearchSuggestions( string searchText )
    {
        ServiceReply<List<string>?> reply = await _productService.GetProductSuggestions( searchText );
        return GetReturnFromReply( reply );
    }
    [HttpGet( "details" )]
    public async Task<ActionResult<ProductDto?>> GetDetails( int productId )
    {
        ServiceReply<ProductDto?> reply = await _productService.GetProductDetails( productId );
        return GetReturnFromReply( reply );
    }
}