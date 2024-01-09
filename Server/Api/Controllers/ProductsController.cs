using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Shared.Products;
using BlazorElectronics.Shared.Products.Search;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class ProductsController : _Controller
{
    readonly IProductServiceCustomer _productServiceCustomer;

    public ProductsController( ILogger<_Controller> logger, IProductServiceCustomer productServiceCustomer )
        : base( logger )
    {
        _productServiceCustomer = productServiceCustomer;
    }
    
    [HttpPost( "search" )]
    public async Task<ActionResult<ProductSummaryDto?>> SearchProducts( [FromBody] ProductSearchRequestDto requestDto )
    {
        ServiceReply<ProductSearchReplyDto?> reply = await _productServiceCustomer.GetSearch( requestDto );
        return GetReturnFromReply( reply );
    }
    [HttpGet( "suggestions" )]
    public async Task<ActionResult<List<string>?>> SearchSuggestions( string searchText )
    {
        ServiceReply<List<string>?> reply = await _productServiceCustomer.GetSuggestions( searchText );
        return GetReturnFromReply( reply );
    }
    [HttpGet( "details" )]
    public async Task<ActionResult<ProductDto?>> GetDetails( int productId )
    {
        ServiceReply<ProductDto?> reply = await _productServiceCustomer.GetDetails( productId );
        return GetReturnFromReply( reply );
    }
}