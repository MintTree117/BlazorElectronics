using BlazorElectronics.Server.Caches.Products;
using BlazorElectronics.Server.Models.Products;
using BlazorElectronics.Server.Repositories.Products;
using BlazorElectronics.Shared.DtosOutbound.Products;
using BlazorElectronics.Shared.Inbound.Products;
using BlazorElectronics.Shared.Mutual;

namespace BlazorElectronics.Server.Services.Products;

public class ProductService : ApiService, IProductService
{
    readonly IProductCache _cache;
    readonly IProductSearchRepository _productSearchRepository;
    readonly IProductDetailsRepository _productDetailsRepository;

    const int MAX_PRODUCT_LIST_ROWS = 100;
    const int MAX_SEARCH_TEXT_LENGTH = 64;
    const int MAX_FILTER_ID_LENGTH = 8;

    public ProductService(
        ILogger logger, IProductCache cache, IProductSearchRepository productSearchRepository, IProductDetailsRepository productDetailsRepository )
        : base( logger )
    {
        _cache = cache;
        _productSearchRepository = productSearchRepository;
        _productDetailsRepository = productDetailsRepository;
    }

    public Task<ApiReply<string?>> GetProductSearchQueryString( ProductSearchRequest request )
    {
        throw new NotImplementedException();
    }
    public async Task<ApiReply<ProductSuggestionsResponse?>> GetProductSuggestions( ProductSuggestionRequest request )
    {
        Task<IEnumerable<string>?> repoFunction = _productSearchRepository.GetSearchSuggestions( request.SearchText!, request.CategoryIdMap!.CategoryTier, request.CategoryIdMap.CategoryId );
        ApiReply<IEnumerable<string>?> repoReply = await ExecuteIoCall( async () => await repoFunction );

        if ( !repoReply.Success )
            return new ApiReply<ProductSuggestionsResponse?>( repoReply.Message );

        return new ApiReply<ProductSuggestionsResponse?> {
            Data = await MapSearchSuggestionsToResponse( repoReply.Data! ),
            Success = true,
            Message = "Found matching results."
        };
    }
    public async Task<ApiReply<ProductSearchResponse?>> GetProductSearch( CategoryIdMap? categoryIdMap, ProductSearchRequest? request )
    {
        request = await ValidateProductSearchRequest( request );
        
        IEnumerable<ProductSearchModel>? models;

        try
        {
            models = await _productSearchRepository.GetProductSearch( categoryIdMap, request );

            if ( models is null )
                return new ApiReply<ProductSearchResponse?>( NO_DATA_FOUND_MESSAGE );
        }
        catch ( ServiceException e )
        {
            _logger.LogError( e.Message, e );
            return new ApiReply<ProductSearchResponse?>( INTERNAL_SERVER_ERROR_MESSAGE );
        }

        return new ApiReply<ProductSearchResponse?>( await MapProductSearchToResponse( models ) );
    }
    public async Task<ApiReply<ProductDetailsResponse?>> GetProductDetails( int productId )
    {
        Task<ProductDetailsResponse?> cacheFetchFunction = _cache.GetProductDetails( productId );
        var cacheFetchResult = ExecuteIoCall( async () => await cacheFetchFunction );
        
        ProductDetailsResponse? dto;

        try
        {
            dto = await _cache.GetProductDetails( productId );
            if ( dto != null )
                return new ApiReply<ProductDetailsResponse?>( dto, true, "Successfully got Product Details from cache." );
        }
        catch ( ServiceException e )
        {
            return new ApiReply<ProductDetailsResponse?>( e.Message );
        }

        ProductOverviewModel? model;

        try
        {
            model = await _productDetailsRepository.GetProductOverview( productId );
        }
        catch ( ServiceException e )
        {
            return new ApiReply<ProductDetailsResponse?>( e.Message );
        }

        dto = await MapProductDetailsToResponse( model! );

        try
        {
            await _cache.CacheProductDetails( dto );
        }
        catch ( ServiceException e )
        {
            return new ApiReply<ProductDetailsResponse?>( dto, true, $"Successfully retrieved ProductDetails from repository, but failed to cache with message: {e.Message}" );
        } 

        return new ApiReply<ProductDetailsResponse?>( dto, true, "Successfully retrieved ProductDetails from repository, and cached." );
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
            request.MaxRating = request.MaxRating is null ? null : Math.Max( request.MaxRating.Value, 0 );

            if ( request.SearchText?.Length > MAX_SEARCH_TEXT_LENGTH )
                request.SearchText.Remove( MAX_SEARCH_TEXT_LENGTH - 1 );

            ProductSearchRequestSpecFilters? specFilters = request.SpecFilters;

            if ( specFilters is null )
                return request;

            TrimExcessFilterLists( specFilters.IntFilters );
            TrimExcessFilterLists( specFilters.StringFilters );
            TrimExcessFilterLists( specFilters.MultiIncludes );
            TrimExcessFilterLists( specFilters.MultiExcludes );
            TrimExcessBoolFilters( specFilters.BoolFilters );

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
    static async Task<ProductDetailsResponse> MapProductDetailsToResponse( ProductOverviewModel productOverviewModel )
    {
        var dto = new ProductDetailsResponse();

        await Task.Run( () =>
        {
            if ( productOverviewModel.Product == null )
                return;
            
            dto.Id = productOverviewModel.Product.ProductId;
            dto.Title = productOverviewModel.Product.ProductTitle;

            if ( productOverviewModel.ProductDescription != null )
            {
                dto.Description = productOverviewModel.ProductDescription.DescriptionBody ?? "No description!";
            }

            foreach ( ProductVariant variant in productOverviewModel.Product.ProductVariants )
            {
                dto.Variants.Add( new ProductVariant_DTO {
                    Id = variant.VariantId,
                    Name = variant.VariantName ?? "No Variant Name!",
                    Price = variant.VariantPriceMain,
                    SalePrice = variant.VariantPriceSale,
                } );
            }

            foreach ( ProductImage image in productOverviewModel.ProductImages )
            {
                dto.Images.Add( new ProductImage_DTO {
                    Url = image.ImageUrl,
                    VariantId = image.ProductVariantId
                } );
            }
        } );

        return dto;
    }

    static void TrimExcessFilterLists( Dictionary<short, List<short>>? filters )
    {
        if ( filters is null )
            return;
        
        List<short> keysToTrim = filters.Keys.Where( k => filters[ k ].Count > MAX_FILTER_ID_LENGTH ).ToList();
        foreach ( short key in keysToTrim )
        {
            filters[ key ] = filters[ key ].Take( MAX_FILTER_ID_LENGTH ).ToList();
        }
        
        List<short> keysToRemove = filters.Keys.Skip( MAX_FILTER_ID_LENGTH ).ToList();
        foreach ( short key in keysToRemove )
        {
            filters.Remove( key );
        }
    }
    static void TrimExcessBoolFilters( Dictionary<short, bool>? filters )
    {
        if ( filters is null || filters.Count <= MAX_FILTER_ID_LENGTH )
            return;
        
        List<KeyValuePair<short, bool>> trimmedFilters = filters.Take( MAX_FILTER_ID_LENGTH ).ToList();
        filters.Clear();
        
        foreach ( KeyValuePair<short, bool> pair in trimmedFilters )
        {
            filters.Add( pair.Key, pair.Value );
        }
    }
}