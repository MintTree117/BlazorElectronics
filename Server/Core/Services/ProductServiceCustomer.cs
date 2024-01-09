using System.Xml.Linq;
using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Server.Core.Models.Products;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.Products;
using BlazorElectronics.Shared.Products.Search;

namespace BlazorElectronics.Server.Core.Services;

public sealed class ProductServiceCustomer : _ApiService, IProductServiceCustomer
{
    readonly IProductSearchRepository _productSearchRepository;
    readonly IProductRepository _productRepository;

    const int MAX_PRODUCT_LIST_ROWS = 100;
    const int MAX_SEARCH_TEXT_LENGTH = 64;
    const int MAX_FILTER_ID_LENGTH = 8;
    
    public ProductServiceCustomer( ILogger<ProductServiceCustomer> logger, IProductSearchRepository productSearchRepository, IProductRepository productRepository )
        : base( logger )
    {
        _productSearchRepository = productSearchRepository;
        _productRepository = productRepository;
    }

    public async Task<ServiceReply<string?>> GetProductSearchQueryString( ProductSearchRequestDto requestDto )
    {
        try
        {
            var reply = await _productSearchRepository.GetSearchQuery( requestDto );
            return new ServiceReply<string?>( reply );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e, e.Message );
            return new ServiceReply<string?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<List<string>?>> GetSuggestions( string searchText )
    {
        try
        {
            IEnumerable<string>? result = await _productSearchRepository.GetSearchSuggestions( searchText );
            return result is not null
                ? new ServiceReply<List<string>?>( result.ToList() )
                : new ServiceReply<List<string>?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e, e.Message );
            return new ServiceReply<List<string>?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<ProductSearchReplyDto?>> GetSearch( ProductSearchRequestDto requestDto )
    {
        try
        {
            IEnumerable<ProductSearchModel>? models = await _productSearchRepository.GetProductSearch( requestDto );
            ProductSearchReplyDto? response = await MapProductSearchToResponse( models );
            
            return response is not null
                ? new ServiceReply<ProductSearchReplyDto?>( response )
                : new ServiceReply<ProductSearchReplyDto?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<ProductSearchReplyDto?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<ProductDto?>> GetDetails( int productId )
    {
        try
        {
            ProductDetailsModel? model = await _productRepository.Get( productId );
            ProductDto? dto = await MapProductToDto( model );

            return dto is not null
                ? new ServiceReply<ProductDto?>( dto )
                : new ServiceReply<ProductDto?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<ProductDto?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<bool>> UpdateProductsReviewData()
    {
        try
        {
            bool result = await _productRepository.UpdateReviewData();
            return result
                ? new ServiceReply<bool>( true )
                : new ServiceReply<bool>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
    }
    
    static async Task<ProductSearchRequestDto> ValidateProductSearchRequest( ProductSearchRequestDto requestDto )
    {
        return await Task.Run( () =>
        {
            requestDto.Page = Math.Max( requestDto.Page, 0 );
            requestDto.Rows = Math.Clamp( requestDto.Rows, 0, MAX_PRODUCT_LIST_ROWS );

            if ( requestDto.SearchText?.Length > MAX_SEARCH_TEXT_LENGTH )
                requestDto.SearchText.Remove( MAX_SEARCH_TEXT_LENGTH - 1 );
            
            if ( requestDto.Filters is null )
                return requestDto;

            ProductFiltersDto filtersDto = requestDto.Filters;

            filtersDto.MinPrice = filtersDto.MinPrice is null ? null : Math.Max( filtersDto.MinPrice.Value, 0 );
            filtersDto.MaxPrice = filtersDto.MaxPrice is null ? null : Math.Max( filtersDto.MaxPrice.Value, 0 );
            filtersDto.MinRating = filtersDto.MinRating is null ? null : Math.Max( filtersDto.MinRating.Value, 0 );
            
            return requestDto;
        } );
    }
    static async Task<ProductSearchReplyDto?> MapProductSearchToResponse( IEnumerable<ProductSearchModel>? models )
    {
        if ( models is null )
            return null;
        
        return await Task.Run( () =>
        {
            ProductSearchReplyDto dto = new();
            
            foreach ( ProductSearchModel p in models )
            {
                dto.TotalMatches = p.TotalCount;
                
                List<int> categoryIds = new();
                foreach ( var c in p.CategoryIds.Split( "," ).ToList() )
                {
                    if ( int.TryParse( c, out int value ) )
                        categoryIds.Add( value );
                }

                dto.Products.Add( new ProductSummaryDto
                {
                    Id = p.ProductId,
                    VendorId = p.VendorId,
                    Title = p.Title,
                    Thumbnail = p.Thumbnail,
                    Rating = p.Rating,
                    Price = p.Price,
                    SalePrice = p.SalePrice,
                    NumberSold = p.NumberSold,
                    NumberReviews = p.NumberReviews,
                    Categories = categoryIds
                } );
            }

            return dto;
        } );
    }
    static async Task<ProductDto?> MapProductToDto( ProductDetailsModel? model )
    {
        if ( model?.Product is null )
            return null;

        return await Task.Run( () =>
        {
            ProductDto dto = new()
            {
                ProductId = model.Product.ProductId,
                VendorId = model.Product.VendorId,
                Title = model.Product.Title,
                Thumbnail = model.Product.Thumbnail,
                Price = model.Product.Price,
                SalePrice = model.Product.SalePrice,
                ReleaseDate = model.Product.ReleaseDate,
                IsFeatured = model.Product.IsFeatured,
                Rating = model.Product.Rating,
                NumberReviews = model.Product.NumberReviews,
                NumberSold = model.Product.NumberSold,
                Description = model.Description?.Description ?? string.Empty,
                XmlSpecsAggregated = ParseFromXml( model.XmlSpecs?.XmlSpecs ?? string.Empty )
            };

            foreach ( ProductCategoryModel c in model.Categories )
                dto.Categories.Add( c.CategoryId );

            foreach ( ProductImageModel i in model.Images )
                dto.Images.Add( i.ImageUrl );
            
            foreach ( ProductSpecLookupModel l in model.SpecLookups )
            {
                if ( !dto.LookupSpecs.TryGetValue( l.SpecId, out List<int>? values ) )
                {
                    values = new List<int>();
                    dto.LookupSpecs.Add( l.SpecId, values );
                }

                values.Add( l.SpecValueId );
            }

            return dto;
        } );
    }
    static Dictionary<string, string> ParseFromXml( string xmlString )
    {
        XDocument doc = XDocument.Parse( xmlString );
        Dictionary<string, string> dict = new();

        if ( doc.Root is null )
            return dict;

        foreach ( XElement e in doc.Root.Elements() )
        {
            if ( !dict.ContainsKey( e.Name.LocalName ) )
            {
                dict.Add( e.Name.LocalName, e.Value );
            }
            else
            {
                dict[ e.Name.LocalName ] += ", " + e.Value;
            }
        }
        
        return dict;
    }
}