using BlazorElectronics.Server.Dtos.Specs;
using BlazorElectronics.Server.Services.Categories;
using BlazorElectronics.Server.Services.Products;
using BlazorElectronics.Server.Services.Specs;
using BlazorElectronics.Shared.DtosOutbound.Products;
using BlazorElectronics.Shared.Inbound;
using BlazorElectronics.Shared.Inbound.Products;
using BlazorElectronics.Shared.Mutual;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    readonly IProductService _productService;
    readonly ICategoryService _categoryService;
    readonly ISpecLookupService _specLookupService;
    
    public ProductController( IProductService productService, ICategoryService categoryService, ISpecLookupService specLookupService )
    {
        _productService = productService;
        _categoryService = categoryService;
        _specLookupService = specLookupService;
    }
    
    // TESTING
    [HttpGet( "searchQueryBoth/{categoryUrl}/{searchText}" )]
    public async Task<ActionResult<ApiReply<Products_DTO>>> GetSearchQuery( string categoryUrl, string searchText, [FromQuery] ProductSearchRequest? searchRequest )
    {
        /*searchRequest ??= new ProductSearchFilters();

        Reply<string?> response = await _productService.GetProductSearchQueryString( searchRequest );
        return Ok( response );*/

        return BadRequest();
    }
    [HttpGet( "searchQuery/{categoryUrl}" )]
    public async Task<ActionResult<ApiReply<Products_DTO>>> GetSearchQuery( string categoryUrl, [FromQuery] ProductSearchRequest? searchRequest )
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

    [HttpPost( "suggestions" )]
    public async Task<ActionResult<ApiReply<ProductSearchSuggestions_DTO>>> GetProductSeachSuggestions( [FromBody] ProductSuggestionRequest request )
    {
        ApiReply<bool> validateReply = await ValidateSearchSuggestionRequest( request );

        if ( !validateReply.Data )
            return BadRequest( new ApiReply<ProductSearchSuggestions_DTO>( validateReply.Message ) );
        
        return Ok( await _productService.GetProductSuggestions( request ) );
    }
    [HttpPost( "search/{primaryUrl}" )]
    public async Task<ActionResult<ApiReply<ProductSearchResponse?>>> SearchByCategory( string primaryUrl, [FromBody] ProductSearchRequest? filters )
    {
        return await GetProductSearchResponse( filters, primaryUrl );
    }
    [HttpPost( "search/{primaryUrl}/{secondaryUrl}" )]
    public async Task<ActionResult<ApiReply<ProductSearchResponse?>>> SearchByCategory( string primaryUrl, string secondaryUrl, [FromBody] ProductSearchRequest? filters )
    {
        return await GetProductSearchResponse( filters, primaryUrl, secondaryUrl );
    }
    [HttpPost( "search/{primaryUrl}/{secondaryUrl}/{tertiaryUrl}" )]
    public async Task<ActionResult<ApiReply<ProductSearchResponse?>>> SearchByCategory( string primaryUrl, string secondaryUrl, string tertiaryUrl, [FromBody] ProductSearchRequest? filters )
    {
        return await GetProductSearchResponse( filters, primaryUrl, secondaryUrl, tertiaryUrl );
    }
    [HttpGet( "details/{productId:int}" )]
    public async Task<ActionResult<ApiReply<ProductDetails_DTO?>>> GetProductDetails( int productId )
    {
        return Ok( await _productService.GetProductDetails( productId ) );
    }

    async Task<ActionResult<ApiReply<ProductSearchResponse?>>> GetProductSearchResponse( ProductSearchRequest? request, string primaryUrl, string? secondaryUrl = null, string? tertiaryUrl = null )
    {
        ApiReply<CategoryIdMap?> categoryReply = await ValidateCategoryUrls( primaryUrl, secondaryUrl, tertiaryUrl );

        if ( !categoryReply.Success || categoryReply.Data is null )
            return BadRequest( categoryReply.Message );

        ApiReply<CachedSpecData?> specDataReply = await _specLookupService.GetSpecDataDto();

        if ( !specDataReply.Success || specDataReply.Data is null )
            return Ok( specDataReply.Message );

        return Ok( await _productService.GetProductSearch( categoryReply.Data, request, specDataReply.Data ) );
    }
    async Task<ApiReply<bool>> ValidateSearchSuggestionRequest( ProductSuggestionRequest request )
    {
        if ( string.IsNullOrEmpty( request.SearchText ) )
            return new ApiReply<bool>( "SearchText is empty!" );

        if ( request.SearchText.Length > 64 )
            request.SearchText.Remove( 63 );

        ApiReply<bool> categoryReply = await _categoryService.ValidateCategoryIdMap( request.CategoryIdMap );

        return categoryReply.Success
            ? new ApiReply<bool>( true )
            : new ApiReply<bool>( categoryReply.Message );
    }
    async Task<ApiReply<CategoryIdMap?>> ValidateCategoryUrls( string? primaryUrl, string? secondaryUrl = null, string? tertiaryUrl = null )
    {
        if ( string.IsNullOrEmpty( primaryUrl ) )
            return new ApiReply<CategoryIdMap?>( "Invalid CategoryUrl!" );

        ApiReply<CategoryIdMap?> categoryReply = await _categoryService.GetCategoryIdMapFromUrl( primaryUrl, secondaryUrl, tertiaryUrl );

        return categoryReply.Success
            ? new ApiReply<CategoryIdMap?>( categoryReply.Data )
            : new ApiReply<CategoryIdMap?>( categoryReply.Message );
    }
}