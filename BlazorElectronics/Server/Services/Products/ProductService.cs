using BlazorElectronics.Server.Models.Products;
using BlazorElectronics.Server.Models.Products.Details;
using BlazorElectronics.Server.Repositories.Products;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.DtosOutbound.Products;
using BlazorElectronics.Shared.Outbound.Products;
using BlazorElectronics.Shared.Products;

namespace BlazorElectronics.Server.Services.Products;

public class ProductService : ApiService, IProductService
{
    readonly IProductSearchRepository _productSearchRepository;
    readonly IProductDetailsRepository _productDetailsRepository;

    const int MAX_PRODUCT_LIST_ROWS = 100;
    const int MAX_SEARCH_TEXT_LENGTH = 64;
    const int MAX_FILTER_ID_LENGTH = 8;

    public ProductService(
        ILogger<ApiService> logger, IProductSearchRepository productSearchRepository, IProductDetailsRepository productDetailsRepository )
        : base( logger )
    {
        _productSearchRepository = productSearchRepository;
        _productDetailsRepository = productDetailsRepository;
    }

    public Task<ApiReply<string?>> GetProductSearchQueryString( ProductSearchRequest request )
    {
        throw new NotImplementedException();
    }
    public async Task<ApiReply<ProductSuggestionsResponse?>> GetProductSuggestions( ProductSuggestionRequest request )
    {
        return null;
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
    public async Task<ApiReply<ProductSearchResponse?>> GetProductSearch( CategoryIdMap? categoryIdMap, ProductSearchRequest? request )
    {
        request = await ValidateProductSearchRequest( request );
        
        IEnumerable<ProductSearchModel>? models;

        try
        {
            models = await _productSearchRepository.GetProductSearch( categoryIdMap, request );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ApiReply<ProductSearchResponse?>( ServiceErrorType.ServerError );
        }

        return models is not null 
            ? new ApiReply<ProductSearchResponse?>( await MapProductSearchToResponse( models ) ) 
            : new ApiReply<ProductSearchResponse?>( ServiceErrorType.NotFound );
    }
    public async Task<ApiReply<ProductDetailsResponse?>> GetProductDetails( int productId, CategoriesResponse categoriesResponse )
    {
        ProductDetailsModel? model;

        try
        {
            model = await _productDetailsRepository.GetProductDetails( productId );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ApiReply<ProductDetailsResponse?>( ServiceErrorType.ServerError );
        }

        if ( model is null )
            return new ApiReply<ProductDetailsResponse?>( ServiceErrorType.NotFound );

        ProductDetailsResponse? dto = await MapProductDetailsToResponse( model, categoriesResponse );

        return dto is not null
            ? new ApiReply<ProductDetailsResponse?>( dto )
            : new ApiReply<ProductDetailsResponse?>( ServiceErrorType.NotFound );
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
    static async Task<ProductDetailsResponse?> MapProductDetailsToResponse( ProductDetailsModel detailsModel, CategoriesResponse categoriesResponse )
    {
        return await Task.Run( () =>
        {
            if ( detailsModel.Overview is null )
                return null;
            
            ProductOverviewModel overviewModel = detailsModel.Overview;

            var variants = new List<ProductVariantResponse>();
            var images = new List<ProductImageResponse>();

            foreach ( ProductVariantModel variant in detailsModel.Variants )
            {
                variants.Add( new ProductVariantResponse {
                    Id = variant.VariantId,
                    Name = variant.VariantName,
                    Price = variant.VariantPriceMain,
                    SalePrice = variant.VariantPriceSale,
                } );
            }

            foreach ( ProductImageModel image in detailsModel.Images )
            {
                images.Add( new ProductImageResponse {
                    Url = image.ImageUrl,
                    VariantId = image.VariantId
                } );
            }

            var primaryCategory = new ProductCategoryResponse
            {
                Name = categoriesResponse.Primary[ detailsModel.PrimaryCategory ].Name,
                Url = categoriesResponse.Primary[ detailsModel.PrimaryCategory ].Url
            };

            //List<ProductCategoryResponse>? parsedSecondaryCategories = ParseProductDetailsCategories( detailsModel.SecondaryCategories, categoriesDto.SecondaryIds, categoriesDto.SecondaryResponses );
            //List<ProductCategoryResponse>? parsedTertiaryCategories = ParseProductDetailsCategories( detailsModel.TertiaryCategories, categoriesDto.TertiaryIds, categoriesDto.TertiaryResponses );

            return new ProductDetailsResponse
            {
                PrimaryCategory = primaryCategory,
                //SecondaryCategories = parsedSecondaryCategories ?? new List<ProductCategoryResponse>(),
                //TertiaryCategories = parsedTertiaryCategories ?? new List<ProductCategoryResponse>(),
                Title = overviewModel.Title,
                Rating = overviewModel.Rating,
                ReleaseDate = overviewModel.ReleaseDate,
                HasDrm = overviewModel.HasDrm,
                NumberSold = overviewModel.NumberSold,
                VariantTypeName = "",
                Description = detailsModel.ProductDescription ?? "No description!",
                Images = images,
                Variants = variants
            };
        } );
    }

    static List<ProductCategoryResponse>? ParseProductDetailsCategories( string? modelCategories, IReadOnlyDictionary<short, short> dtoIds, IReadOnlyList<CategoryResponse> dtoResponses )
    {
        if ( string.IsNullOrEmpty( modelCategories ) )
            return null;
        
        List<int>? ids = modelCategories?
            .Split( ',' )
            .ToList()
            .Select( int.Parse )
            .ToList();

        if ( ids is null )
            return new List<ProductCategoryResponse>();

        var responses = new List<ProductCategoryResponse>();

        foreach ( int id in ids )
        {
            if ( !dtoIds.TryGetValue( ( short ) id, out short responseId ) )
                continue;

            responses.Add( new ProductCategoryResponse
            {
                Name = dtoResponses[ responseId ].Name,
                Url = dtoResponses[ responseId ].Url
            } );
        }

        return responses;
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