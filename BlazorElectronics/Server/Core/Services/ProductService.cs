using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Server.Core.Models.Products;
using BlazorElectronics.Server.Data;
using BlazorElectronics.Server.Services;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.Outbound.Products;
using BlazorElectronics.Shared.Products.Search;

namespace BlazorElectronics.Server.Core.Services;

public class ProductService : ApiService, IProductService
{
    readonly IProductSearchRepository _productSearchRepository;
    readonly IProductRepository _productRepository;

    const int MAX_PRODUCT_LIST_ROWS = 100;
    const int MAX_SEARCH_TEXT_LENGTH = 64;
    const int MAX_FILTER_ID_LENGTH = 8;

    public ProductService( ILogger<ApiService> logger, IProductSearchRepository productSearchRepository, IProductRepository productRepository )
        : base( logger )
    {
        _productSearchRepository = productSearchRepository;
        _productRepository = productRepository;
    }

    public async Task<ServiceReply<string?>> GetProductSearchQueryString( ProductSearchRequest request )
    {
        try
        {
            var reply = await _productSearchRepository.GetProductSearchQuery( request );
            return new ServiceReply<string?>( reply );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e, e.Message );
            return new ServiceReply<string?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<ProductSuggestionsResponse?>> GetProductSuggestions( ProductSuggestionRequest request )
    {
        return new ServiceReply<ProductSuggestionsResponse?>( null );
        /*Task<IEnumerable<string>?> repoFunction = _productSearchRepository.GetSearchSuggestions( request.SearchText!, request.CategoryIdMap!.CategoryType, request.CategoryIdMap.CategoryId );
        ApiReply<IEnumerable<string>?> repoReply = await ExecuteIoCall( async () => await repoFunction );

        if ( !repoReply.Success )
            return new ApiReply<ProductSuggestionsResponse?>( repoReply.Message );

        return new ApiReply<ProductSuggestionsResponse?> {
            Data = await MapSearchSuggestionsToResponse( repoReply.Data! ),
            Success = true,
            Message = "Found matching results."
        };*/
    }
    public async Task<ServiceReply<ProductSearchResponse?>> GetProductSearch( ProductSearchRequest request )
    {
        try
        {
            IEnumerable<ProductSearchModel>? models = await _productSearchRepository.GetProductSearch( request );
            ProductSearchResponse? response = await MapProductSearchToResponse( models );

            return response is not null
                ? new ServiceReply<ProductSearchResponse?>( response )
                : new ServiceReply<ProductSearchResponse?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<ProductSearchResponse?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<ProductDetailsResponse?>> GetProductDetails( int productId, CategoryData categoryData )
    {
        return new ServiceReply<ProductDetailsResponse?>();
    }

    static async Task<ProductSearchRequest> ValidateProductSearchRequest( ProductSearchRequest request )
    {
        return await Task.Run( () =>
        {
            request.Page = Math.Max( request.Page, 0 );
            request.Rows = Math.Clamp( request.Rows, 0, MAX_PRODUCT_LIST_ROWS );

            if ( request.SearchText?.Length > MAX_SEARCH_TEXT_LENGTH )
                request.SearchText.Remove( MAX_SEARCH_TEXT_LENGTH - 1 );
            
            if ( request.Filters is null )
                return request;

            ProductSearchFilters filters = request.Filters;

            filters.MinPrice = filters.MinPrice is null ? null : Math.Max( filters.MinPrice.Value, 0 );
            filters.MaxPrice = filters.MaxPrice is null ? null : Math.Max( filters.MaxPrice.Value, 0 );
            filters.MinRating = filters.MinRating is null ? null : Math.Max( filters.MinRating.Value, 0 );
            
            return request;
        } );
    }
    static async Task<ProductSuggestionsResponse> MapSearchSuggestionsToResponse( IEnumerable<string> suggestions )
    {
        var suggestionList = new List<string>();

        await Task.Run( () =>
        {
            suggestionList = suggestions.ToList();
        } );

        return new ProductSuggestionsResponse {
            Suggestions = suggestionList
        };
    }
    static async Task<ProductSearchResponse?> MapProductSearchToResponse( IEnumerable<ProductSearchModel>? models )
    {
        if ( models is null )
            return null;
        
        return await Task.Run( () =>
        {
            ProductSearchResponse dto = new();
            
            foreach ( ProductSearchModel p in models )
            {
                dto.TotalMatches = p.TotalCount;

                dto.Products.Add( new ProductResponse
                {
                    Id = p.ProductId,
                    Title = p.Title,
                    Thumbnail = p.Thumbnail,
                    Rating = p.Rating,
                    Price = p.Price,
                    SalePrice = p.SalePrice,
                    NumberSold = p.NumberSold,
                    NumberReviews = p.NumberReviews
                } );
            }

            return dto;
        } );
    }
}