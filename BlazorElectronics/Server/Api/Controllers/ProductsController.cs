using BlazorElectronics.Server.Api.Interfaces;
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
    public async Task<ActionResult<ProductResponse?>> SearchQuery()
    {
        ProductSearchRequest r = new()
        {
            CategoryId = 1
        };
        ServiceReply<string?> reply = await _productService.GetProductSearchQueryString( r );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "search" )]
    public async Task<ActionResult<ProductResponse?>> SearchProducts( [FromBody] ProductSearchRequest request )
    {
        ServiceReply<ProductSearchResponse?> reply = await _productService.GetProductSearch( request );
        return GetReturnFromReply( reply );
    }
}