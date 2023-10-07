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

    [HttpGet( "searchQuery/{categoryUrl}" )]
    public async Task<ActionResult<ServiceResponse<Products_DTO>>> GetSearchQuery( string categoryUrl, [FromQuery] ProductSearchFilters_DTO searchFilters )
    {
        ServiceResponse<string?> response = await _productService.TestGetQueryString( categoryUrl, searchFilters );
        return Ok( response );
    }
    [HttpGet( "products" )]
    public async Task<ActionResult<ServiceResponse<Products_DTO>>> GetAllProducts()
    {
        ServiceResponse<Products_DTO?> response = await _productService.GetProducts();
        return Ok( response );
    }
    [HttpGet( "{categoryUrl}" )]
    public async Task<ActionResult<ServiceResponse<ProductSearch_DTO>>> SearchProducts( string categoryUrl, [FromQuery] ProductSearchFilters_DTO? filters )
    {
        ServiceResponse<ProductSearch_DTO?> response = await _productService.SearchProducts( categoryUrl, filters );
        return Ok( response );
    }
    [HttpGet( "details/{productId:int}" )]
    public async Task<ActionResult<ServiceResponse<ProductDetails_DTO?>>> GetProductDetails( int productId )
    {
        ServiceResponse<ProductDetails_DTO?> response = await _productService.GetProductDetails( productId );
        return Ok( response );
    }
}