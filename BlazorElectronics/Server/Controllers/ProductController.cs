using Microsoft.AspNetCore.Mvc;
using BlazorElectronics.Server.Services.Products;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DtosInbound.Products;
using BlazorElectronics.Shared.DtosOutbound.Products;

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
    public async Task<ActionResult<ServiceResponse<Products_DTO>>> GetSearchQuery( string categoryUrl, string searchText, [FromQuery] ProductSearchRequest_DTO? searchRequest )
    {
        searchRequest ??= new ProductSearchRequest_DTO();
        searchRequest.CategoryUrl = categoryUrl;
        searchRequest.SearchText = searchText;

        ServiceResponse<string?> response = await _productService.GetProductSearchQueryString( searchRequest );
        return Ok( response );
    }
    [HttpGet( "searchQuery/{categoryUrl}" )]
    public async Task<ActionResult<ServiceResponse<Products_DTO>>> GetSearchQuery( string categoryUrl, [FromQuery] ProductSearchRequest_DTO? searchRequest )
    {
        searchRequest ??= new ProductSearchRequest_DTO();
        searchRequest.CategoryUrl = categoryUrl;
        
        ServiceResponse<string?> response = await _productService.GetProductSearchQueryString( searchRequest );
        return Ok( response );
    }
    [HttpGet( "search-suggestions/{searchText}" )]
    public async Task<ActionResult<ServiceResponse<ProductSearchSuggestions_DTO>>> GetProductSeachSuggestions( string searchText )
    {
        ServiceResponse<ProductSearchSuggestions_DTO?> response = await _productService.GetTextSearchSuggestions( searchText );
        return Ok( response );
    }
    [HttpGet( "products" )]
    public async Task<ActionResult<ServiceResponse<Products_DTO>>> GetAllProducts()
    {
        ServiceResponse<Products_DTO?> response = await _productService.GetAllProducts();
        return Ok( response );
    }
    
    [HttpGet( "search-category-text/{categoryUrl}/{searchText}" )]
    public async Task<ActionResult<ServiceResponse<ProductSearchResults_DTO>>> SearchProductsByCategoryAndText( string categoryUrl, string searchText, [FromQuery] ProductSearchRequest_DTO? filters )
    {
        filters ??= new ProductSearchRequest_DTO();
        filters.CategoryUrl = categoryUrl;
        filters.SearchText = searchText;
        
        ServiceResponse<ProductSearchResults_DTO?> response = await _productService.GetProductSearch( filters );
        return Ok( response );
    }
    [HttpGet( "search-category/{categoryUrl}" )]
    public async Task<ActionResult<ServiceResponse<ProductSearchResults_DTO>>> SearchProductsByCategory( string categoryUrl, [FromQuery] ProductSearchRequest_DTO? filters )
    {
        filters ??= new ProductSearchRequest_DTO();
        filters.CategoryUrl = categoryUrl;
        
        ServiceResponse<ProductSearchResults_DTO?> response = await _productService.GetProductSearch( filters );
        return Ok( response );
    }
    [HttpGet( "search-text/{searchText}" )]
    public async Task<ActionResult<ServiceResponse<ProductSearchResults_DTO>>> SearchProductsByText( string searchText, [FromQuery] ProductSearchRequest_DTO? filters )
    {
        filters ??= new ProductSearchRequest_DTO();
        filters.SearchText = searchText;

        ServiceResponse<ProductSearchResults_DTO?> response = await _productService.GetProductSearch( filters );
        return Ok( response );
    }


    [HttpGet( "search" )]
    public async Task<ActionResult<ServiceResponse<ProductSearchResults_DTO>>> SearchProducts( [FromQuery] ProductSearchRequest_DTO? filters )
    {
        ServiceResponse<ProductSearchResults_DTO?> response = await _productService.GetProductSearch( filters );
        return Ok( response );
    }
    [HttpGet( "details/{productId:int}" )]
    public async Task<ActionResult<ServiceResponse<ProductDetails_DTO?>>> GetProductDetails( int productId )
    {
        ServiceResponse<ProductDetails_DTO?> response = await _productService.GetProductDetails( productId );
        return Ok( response );
    }
}