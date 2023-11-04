using BlazorElectronics.Server.Services.Categories;
using Microsoft.AspNetCore.Mvc;
using BlazorElectronics.Server.Services.Products;
using BlazorElectronics.Shared.DtosOutbound.Products;
using BlazorElectronics.Shared.Inbound.Products;
using BlazorElectronics.Shared.Mutual;

namespace BlazorElectronics.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    readonly IProductService _productService;
    readonly ICategoryService _categoryService;
    
    public ProductController( IProductService productService, ICategoryService categoryService )
    {
        _productService = productService;
        _categoryService = categoryService;
    }
    
    // TESTING
    [HttpGet( "searchQueryBoth/{categoryUrl}/{searchText}" )]
    public async Task<ActionResult<Reply<Products_DTO>>> GetSearchQuery( string categoryUrl, string searchText, [FromQuery] ProductSearchRequest? searchRequest )
    {
        /*searchRequest ??= new ProductSearchFilters();

        Reply<string?> response = await _productService.GetProductSearchQueryString( searchRequest );
        return Ok( response );*/

        return BadRequest();
    }
    [HttpGet( "searchQuery/{categoryUrl}" )]
    public async Task<ActionResult<Reply<Products_DTO>>> GetSearchQuery( string categoryUrl, [FromQuery] ProductSearchRequest? searchRequest )
    {
        /*if ( searchRequest == null )
            return BadRequest( new Reply<Products_DTO>( "Search request is null!" ) );

        Reply<int> categoryResponse = await _categoryService.GetCategoryIdMapFromUrl( categoryUrl );

        if ( !categoryResponse.Success )
            return BadRequest( new Reply<Products_DTO>( "Invalid category!" ) );

        Reply<string?> response = await _productService.GetProductSearchQueryString( searchRequest );
        return Ok( response );*/
        return BadRequest();
    }
    // END TESTING 

    [HttpGet( "all" )]
    public async Task<ActionResult<Reply<Products_DTO>>> GetAllProducts()
    {
        Reply<Products_DTO?> reply = await _productService.GetAllProducts();
        return Ok( reply );
    }

    [HttpPost( "search-suggestions" )]
    public async Task<ActionResult<Reply<ProductSearchSuggestions_DTO>>> GetProductSeachSuggestions( [FromBody] ProductSuggestionRequest request )
    {
        Reply<bool> validateReply = await ValidateSearchSuggestionRequest( request );

        if ( !validateReply.Data )
            return BadRequest( new Reply<ProductSearchSuggestions_DTO>( validateReply.Message ) );
        
        return Ok( await _productService.GetProductSuggestions( request ) );
    }
    [HttpGet( "search/{primaryUrl}" )]
    public async Task<ActionResult<Reply<ProductSearchResults_DTO?>>> SearchByPrimaryCategory( string primaryUrl, [FromQuery] ProductSearchRequest? filters )
    {
        return await GetProductSearchResponse( filters, primaryUrl );
    }
    [HttpGet( "search/{primaryUrl}/{secondaryUrl}" )]
    public async Task<ActionResult<Reply<ProductSearchResults_DTO?>>> SearchByPrimaryCategory( string primaryUrl, string secondaryUrl, [FromQuery] ProductSearchRequest? filters )
    {
        return await GetProductSearchResponse( filters, primaryUrl, secondaryUrl );
    }
    [HttpGet( "search/{primaryUrl}/{secondaryUrl}/{tertiaryUrl}" )]
    public async Task<ActionResult<Reply<ProductSearchResults_DTO?>>> SearchByPrimaryCategory( string primaryUrl, string secondaryUrl, string tertiaryUrl, [FromQuery] ProductSearchRequest? filters )
    {
        return await GetProductSearchResponse( filters, primaryUrl, secondaryUrl, tertiaryUrl );
    }
    [HttpGet( "details/{productId:int}" )]
    public async Task<ActionResult<Reply<ProductDetails_DTO?>>> GetProductDetails( int productId )
    {
        return Ok( await _productService.GetProductDetails( productId ) );
    }

    async Task<ActionResult<Reply<ProductSearchResults_DTO?>>> GetProductSearchResponse( ProductSearchRequest? request, string primaryUrl, string? secondaryUrl = null, string? tertiaryUrl = null )
    {
        Reply<CategoryIdMap?> categoryReply = await ValidateCategoryUrls( primaryUrl, secondaryUrl, tertiaryUrl );

        if ( !categoryReply.Success )
            return BadRequest( categoryReply.Message );

        return Ok( await _productService.GetProductSearch( categoryReply.Data!, request ) );
    }
    async Task<Reply<bool>> ValidateSearchSuggestionRequest( ProductSuggestionRequest request )
    {
        if ( string.IsNullOrEmpty( request.SearchText ) )
            return new Reply<bool>( "SearchText is empty!" );

        if ( request.SearchText.Length > 64 )
            request.SearchText.Remove( 63 );

        Reply<bool> categoryReply = await _categoryService.ValidateCategoryIdMap( request.CategoryIdMap );

        return categoryReply.Success
            ? new Reply<bool>( categoryReply.Message )
            : new Reply<bool>( true, true, categoryReply.Message! );
    }
    async Task<Reply<CategoryIdMap?>> ValidateCategoryUrls( string? primaryUrl, string? secondaryUrl = null, string? tertiaryUrl = null )
    {
        if ( string.IsNullOrEmpty( primaryUrl ) )
            return new Reply<CategoryIdMap?>( "Invalid CategoryUrl!" );

        Reply<CategoryIdMap?> categoryReply = await _categoryService.GetCategoryIdMapFromUrl( primaryUrl, secondaryUrl, tertiaryUrl );

        return categoryReply.Success
            ? new Reply<CategoryIdMap?>( categoryReply.Data!, true, categoryReply.Message! )
            : new Reply<CategoryIdMap?>( categoryReply.Message );
    }
}