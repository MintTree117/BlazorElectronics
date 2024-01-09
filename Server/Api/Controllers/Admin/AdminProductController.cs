using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Shared.Products;
using BlazorElectronics.Shared.Products.Search;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Api.Controllers.Admin;

[Route( "api/[controller]" )]
[ApiController]
public class AdminProductController : _AdminController
{
    readonly IProductServiceAdmin _adminProductService;
    
    public AdminProductController( ILogger<_UserController> logger, IUserAccountService userAccountService, ISessionService sessionService, IProductServiceAdmin adminProductService )
        : base( logger, userAccountService, sessionService )
    {
        _adminProductService = adminProductService;
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
                SpecsInclude = new Dictionary<int, List<int>>() { { 1, new List<int>() { 1 } } }
            }
        };
        ServiceReply<string?> reply = await _adminProductService.GetProductSearchQueryString( r );
        return GetReturnFromReply( reply );
    }
    [HttpGet( "get-edit" )]
    public async Task<ActionResult<ProductEditDto?>> GetProductEdit( int itemId )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<ProductEditDto?> reply = await _adminProductService.GetEdit( itemId );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "add" )]
    public async Task<ActionResult<int>> AddProduct( [FromBody] ProductEditDto requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<int> reply = await _adminProductService.Add( requestDto );
        return GetReturnFromReply( reply );
    }
    [HttpPut( "update" )]
    public async Task<ActionResult<bool>> UpdateUpdateProduct( [FromBody] ProductEditDto requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<bool> reply = await _adminProductService.Update( requestDto );
        return GetReturnFromReply( reply );
    }
    [HttpDelete( "remove" )]
    public async Task<ActionResult<bool>> RemoveProduct( int itemId )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<bool> reply = await _adminProductService.Remove( itemId );
        return GetReturnFromReply( reply );
    }
}