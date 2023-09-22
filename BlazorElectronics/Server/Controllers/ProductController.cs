using Microsoft.AspNetCore.Mvc;
using BlazorElectronics.Server.Services;
using BlazorElectronics.Server.Services.Products;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DataTransferObjects.Products;

namespace BlazorElectronics.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    readonly IProductService _productService;
    
    public ProductController( IProductService productService )
    {
        _productService = productService;
    }

    [HttpGet("products")]
    public async Task<ActionResult<ControllerResponse<ProductList_DTO>>> GetProducts( [FromQuery] ProductSearchFilters_DTO searchFilters )
    {
        ServiceResponse<ProductList_DTO?> response = await _productService.GetProducts();

        if ( response == null )
            return new ActionResult<ControllerResponse<ProductList_DTO>>( 
                new ControllerResponse<ProductList_DTO>( null, false, "Service response is null!" ) );
        
        return Ok( new ControllerResponse<ProductList_DTO>( response.Data, response.Success, response.Message ) );
    }
    [HttpGet("product_details/{productId:int}")]
    public async Task<ActionResult<ControllerResponse<ProductDetails_DTO>>> GetProductDetails( int productId )
    {
        ServiceResponse<ProductDetails_DTO?>? response = await _productService.GetProductDetails( productId );

        if ( response == null )
            return new ActionResult<ControllerResponse<ProductDetails_DTO>>(
                new ControllerResponse<ProductDetails_DTO>( null, false, "Service response is null!" ) );

        return Ok( new ControllerResponse<ProductDetails_DTO>( response.Data, response.Success, response.Message ) );
    }
}