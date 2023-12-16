using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Shared.Products;
using BlazorElectronics.Shared.Products.Search;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : _Controller
{
    readonly IProductService _productService;

    public ProductsController( ILogger<_Controller> logger, IProductService productService )
        : base( logger )
    {
        _productService = productService;
    }

    [HttpGet( "search-query" )]
    public async Task<ActionResult<ProductSummaryResponse?>> SearchQuery()
    {
        ProductSearchRequest r = new()
        {
            CategoryId = 1,
            Filters = new ProductSearchFilters
            {
                SpecsInclude = new Dictionary<int, List<int>>() { { 1, new List<int>(){ 1 } } }
            }
        };
        ServiceReply<string?> reply = await _productService.GetProductSearchQueryString( r );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "search" )]
    public async Task<ActionResult<ProductSummaryResponse?>> SearchProducts( [FromBody] ProductSearchRequest request )
    {
        ServiceReply<ProductSearchResponse?> reply = await _productService.GetProductSearch( request );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "suggestions" )]
    public async Task<ActionResult<List<string>?>> SearchSuggestions( [FromBody] string searchText )
    {
        ServiceReply<List<string>?> reply = await _productService.GetProductSuggestions( searchText );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "details" )]
    public async Task<ActionResult<ProductDto?>> GetDetails( [FromBody] int productId )
    {
        ServiceReply<ProductDto?> reply = await _productService.GetProductDetails( productId );
        return GetReturnFromReply( reply );
    }
}