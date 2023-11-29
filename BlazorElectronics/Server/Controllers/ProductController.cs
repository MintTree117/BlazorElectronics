using BlazorElectronics.Server.Repositories.SpecLookups;
using BlazorElectronics.Server.Services.Categories;
using BlazorElectronics.Server.Services.Products;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Outbound.Products;
using BlazorElectronics.Shared.Products;
using BlazorElectronics.Shared.Products.Search;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    readonly IProductService _productService;
    readonly ICategoryService _categoryService;
    readonly ISpecLookupRepository _repository;
    
    public ProductController( IProductService productService, ICategoryService categoryService, ISpecLookupRepository specLookupRepository )
    {
        _productService = productService;
        _categoryService = categoryService;
        _repository = specLookupRepository;
    }

    [HttpPost( "suggestions" )]
    public async Task<ActionResult<ApiReply<ProductSuggestionsResponse>>> GetProductSeachSuggestions( [FromBody] ProductSuggestionRequest request )
    {
        ApiReply<bool> validateReply = await ValidateSearchSuggestionRequest( request );

        if ( !validateReply.Data )
            return BadRequest( new ApiReply<ProductSuggestionsResponse>( ServiceErrorType.NotFound, validateReply.Message ) );
        
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
    public async Task<ActionResult<ApiReply<ProductDetailsResponse?>>> GetProductDetails( int productId )
    {
        ApiReply<CategoriesResponse?> categoriesReply = await _categoryService.GetCategoriesResponse();

        if ( !categoriesReply.Success || categoriesReply.Data is null )
            return Ok( new ApiReply<ProductDetailsResponse?>( ServiceErrorType.NotFound, categoriesReply.Message ) );
        
        return Ok( await _productService.GetProductDetails( productId, categoriesReply.Data ) );
    }

    async Task<ActionResult<ApiReply<ProductSearchResponse?>>> GetProductSearchResponse( ProductSearchRequest? request, string primaryUrl, string? secondaryUrl = null, string? tertiaryUrl = null )
    {
        ApiReply<CategoryIdMap?> categoryReply = await ValidateCategoryUrls( primaryUrl, secondaryUrl, tertiaryUrl );

        if ( !categoryReply.Success || categoryReply.Data is null )
            return BadRequest( categoryReply.Message );

        return Ok( await _productService.GetProductSearch( categoryReply.Data, request ) );
    }
    async Task<ApiReply<bool>> ValidateSearchSuggestionRequest( ProductSuggestionRequest request )
    {
        if ( string.IsNullOrEmpty( request.SearchText ) )
            return new ApiReply<bool>( ServiceErrorType.NotFound, "SearchText is empty!" );

        if ( request.SearchText.Length > 64 )
            request.SearchText.Remove( 63 );

        ApiReply<bool> categoryReply = await _categoryService.ValidateCategoryIdMap( request.CategoryIdMap );

        return categoryReply.Success
            ? new ApiReply<bool>( true )
            : new ApiReply<bool>( categoryReply.ErrorType, categoryReply.Message );
    }
    async Task<ApiReply<CategoryIdMap?>> ValidateCategoryUrls( string? primaryUrl, string? secondaryUrl = null, string? tertiaryUrl = null )
    {
        if ( string.IsNullOrEmpty( primaryUrl ) )
            return new ApiReply<CategoryIdMap?>( ServiceErrorType.ValidationError, "Invalid CategoryUrl!" );

        ApiReply<CategoryIdMap?> categoryReply = await _categoryService.GetCategoryIdMapFromUrl( primaryUrl, secondaryUrl, tertiaryUrl );

        return categoryReply.Success
            ? new ApiReply<CategoryIdMap?>( categoryReply.Data )
            : new ApiReply<CategoryIdMap?>( categoryReply.ErrorType, categoryReply.Message );
    }
}