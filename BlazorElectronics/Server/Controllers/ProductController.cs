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

    [HttpGet( "searchQueryBoth/{categoryUrl}/{searchText}" )]
    public async Task<ActionResult<ServiceResponse<Products_DTO>>> GetSearchQuery( string categoryUrl, string searchText, [FromQuery] ProductSearchFilters_DTO? searchFilters )
    {
        searchFilters ??= new ProductSearchFilters_DTO();
        searchFilters.CategoryUrl = categoryUrl;
        searchFilters.SearchText = searchText;

        ServiceResponse<string?> response = await _productService.TestGetQueryString( searchFilters );
        return Ok( response );
    }
    [HttpGet( "searchQuery/{categoryUrl}" )]
    public async Task<ActionResult<ServiceResponse<Products_DTO>>> GetSearchQuery( string categoryUrl, [FromQuery] ProductSearchFilters_DTO? searchFilters )
    {
        searchFilters ??= new ProductSearchFilters_DTO();
        searchFilters.CategoryUrl = categoryUrl;
        
        ServiceResponse<string?> response = await _productService.TestGetQueryString( searchFilters );
        return Ok( response );
    }
    [HttpGet( "search-suggestions/{searchText}" )]
    public async Task<ActionResult<ServiceResponse<ProductSearchSuggestions_DTO>>> GetProductSeachSuggestions( string searchText )
    {
        ServiceResponse<ProductSearchSuggestions_DTO?> response = await _productService.GetProductSearchSuggestions( searchText );
        return Ok( response );
    }
    [HttpGet( "products" )]
    public async Task<ActionResult<ServiceResponse<Products_DTO>>> GetAllProducts()
    {
        ServiceResponse<Products_DTO?> response = await _productService.GetProducts();
        return Ok( response );
    }
    
    [HttpGet( "search-category-text/{categoryUrl}/{searchText}" )]
    public async Task<ActionResult<ServiceResponse<ProductSearch_DTO>>> SearchProductsByCategoryAndText( string categoryUrl, string searchText, [FromQuery] ProductSearchFilters_DTO? filters )
    {
        filters ??= new ProductSearchFilters_DTO();
        filters.CategoryUrl = categoryUrl;
        filters.SearchText = searchText;
        
        ServiceResponse<ProductSearch_DTO?> response = await _productService.SearchProducts( filters );
        return Ok( response );
    }
    [HttpGet( "search-category/{categoryUrl}" )]
    public async Task<ActionResult<ServiceResponse<ProductSearch_DTO>>> SearchProductsByCategory( string categoryUrl, [FromQuery] ProductSearchFilters_DTO? filters )
    {
        filters ??= new ProductSearchFilters_DTO();
        filters.CategoryUrl = categoryUrl;
        
        ServiceResponse<ProductSearch_DTO?> response = await _productService.SearchProducts( filters );
        return Ok( response );
    }
    [HttpGet( "search-text/{searchText}" )]
    public async Task<ActionResult<ServiceResponse<ProductSearch_DTO>>> SearchProductsByText( string searchText, [FromQuery] ProductSearchFilters_DTO? filters )
    {
        filters ??= new ProductSearchFilters_DTO();
        filters.SearchText = searchText;

        ServiceResponse<ProductSearch_DTO?> response = await _productService.SearchProducts( filters );
        return Ok( response );
    }


    [HttpGet( "search" )]
    public async Task<ActionResult<ServiceResponse<ProductSearch_DTO>>> SearchProducts( [FromQuery] ProductSearchFilters_DTO? filters )
    {
        ServiceResponse<ProductSearch_DTO?> response = await _productService.SearchProducts( filters );
        return Ok( response );
    }
    [HttpGet( "details/{productId:int}" )]
    public async Task<ActionResult<ServiceResponse<ProductDetails_DTO?>>> GetProductDetails( int productId )
    {
        ServiceResponse<ProductDetails_DTO?> response = await _productService.GetProductDetails( productId );
        return Ok( response );
    }
}