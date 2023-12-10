using BlazorElectronics.Server.Models.Products;
using BlazorElectronics.Server.Repositories.Products;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.Outbound.Products;
using BlazorElectronics.Shared.Products;
using BlazorElectronics.Shared.Products.Search;

namespace BlazorElectronics.Server.Services.Products;

public class ProductService : ApiService, IProductService
{
    readonly IProductSearchRepository _productSearchRepository;
    readonly IProductRepository _productRepository;

    const int MAX_PRODUCT_LIST_ROWS = 100;
    const int MAX_SEARCH_TEXT_LENGTH = 64;
    const int MAX_FILTER_ID_LENGTH = 8;

    public ProductService(
        ILogger<ApiService> logger, IProductSearchRepository productSearchRepository, IProductRepository productRepository )
        : base( logger )
    {
        _productSearchRepository = productSearchRepository;
        _productRepository = productRepository;
    }

    public Task<ServiceReply<string?>> GetProductSearchQueryString( ProductSearchRequest request )
    {
        throw new NotImplementedException();
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
    public async Task<ServiceReply<ProductSearchResponse?>> GetProductSearch( int? categoryId, ProductSearchRequest? request )
    {
        request = await ValidateProductSearchRequest( request );
        
        IEnumerable<ProductSearchModel>? models;

        try
        {
            models = await _productSearchRepository.GetProductSearch( categoryId, request );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<ProductSearchResponse?>( ServiceErrorType.ServerError );
        }

        return models is not null 
            ? new ServiceReply<ProductSearchResponse?>( await MapProductSearchToResponse( models ) ) 
            : new ServiceReply<ProductSearchResponse?>( ServiceErrorType.NotFound );
    }
    public async Task<ServiceReply<ProductDetailsResponse?>> GetProductDetails( int productId, CategoryData categoryData )
    {
        return new ServiceReply<ProductDetailsResponse?>();
    }

    static async Task<ProductSearchRequest> ValidateProductSearchRequest( ProductSearchRequest? request )
    {
        if ( request is null )
            return new ProductSearchRequest();

        return await Task.Run( () =>
        {
            request.Page = Math.Max( request.Page, 0 );
            request.Rows = Math.Clamp( request.Rows, 0, MAX_PRODUCT_LIST_ROWS );

            request.MinPrice = request.MinPrice is null ? null : Math.Max( request.MinPrice.Value, 0 );
            request.MaxPrice = request.MaxPrice is null ? null : Math.Max( request.MaxPrice.Value, 0 );
            request.MinRating = request.MinRating is null ? null : Math.Max( request.MinRating.Value, 0 );
            
            if ( request.SearchText?.Length > MAX_SEARCH_TEXT_LENGTH )
                request.SearchText.Remove( MAX_SEARCH_TEXT_LENGTH - 1 );

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
    static async Task<ProductSearchResponse> MapProductSearchToResponse( IEnumerable<ProductSearchModel> models )
    {
        var dto = new ProductSearchResponse();

        await Task.Run( () =>
        {
            foreach ( ProductSearchModel p in models )
            {
                dto.Products.Add( new ProductResponse
                {
                    Id = p.ProductId,
                    Title = p.ProductTitle,
                    Thumbnail = p.ProductThumbnail,
                    Rating = p.ProductRating
                } );
            }
        } );

        return dto;
    }
}