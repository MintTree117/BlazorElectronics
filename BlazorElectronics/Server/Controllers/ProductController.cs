using Microsoft.AspNetCore.Mvc;
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

    [HttpGet( "searchQuery" )]
    public async Task<ActionResult<DtoResponse<Products_DTO>>> GetSearchQuery( [FromQuery] ProductSearchFilters_DTO searchFilters )
    {
        DtoResponse<string?> response = await _productService.TestGetQueryString( searchFilters );
        return Ok( response );
    }
    
    [HttpGet("products")]
    public async Task<ActionResult<DtoResponse<Products_DTO>>> GetProducts( [FromQuery] ProductSearchFilters_DTO? searchFilters )
    {
        DtoResponse<Products_DTO?> response = await _productService.GetProducts( searchFilters );
        return Ok( response );
    }
    [HttpGet("product_details/{productId:int}")]
    public async Task<ActionResult<DtoResponse<ProductDetails_DTO>>> GetProductDetails( int productId )
    {
        DtoResponse<ProductDetails_DTO?> response = await _productService.GetProductDetails( productId );
        return Ok( response );
    }
}