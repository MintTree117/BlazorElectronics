using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Server.Core.Models.Products;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.Products;
using BlazorElectronics.Shared.Products.Search;

namespace BlazorElectronics.Server.Core.Services;

public sealed class ProductServiceAdmin : _ApiService, IProductServiceAdmin
{
    readonly IProductRepository _productRepository;

    public ProductServiceAdmin( ILogger<ProductServiceAdmin> logger, IProductRepository productRepository )
        : base( logger )
    {
        _productRepository = productRepository;
    }
    
    public async Task<ServiceReply<ProductEditDto?>> GetEdit( int id )
    {
        try
        {
            ProductDetailsModel? m = await _productRepository.Get( id );
            ProductEditDto? dto = MapDetailsToEdit( m );

            return dto is not null
                ? new ServiceReply<ProductEditDto?>( dto )
                : new ServiceReply<ProductEditDto?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<ProductEditDto?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<int>> Add( ProductEditDto dto )
    {
        try
        {
            int id = await _productRepository.Insert( MapEditToModel( dto ) );

            return id > 0
                ? new ServiceReply<int>( id )
                : new ServiceReply<int>( ServiceErrorType.Conflict );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<int>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<bool>> Update( ProductEditDto dto )
    {
        try
        {
            var success = await _productRepository.Update( MapEditToModel( dto ) );

            return success
                ? new ServiceReply<bool>( true )
                : new ServiceReply<bool>( ServiceErrorType.Conflict );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<bool>> Remove( int id )
    {
        try
        {
            var success = await _productRepository.Delete( id );

            return success
                ? new ServiceReply<bool>( true )
                : new ServiceReply<bool>( ServiceErrorType.Conflict );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<List<int>>> GetAllIds()
    {
        try
        {
            IEnumerable<int>? ids = await _productRepository.GetIds();
            return ids is not null
                ? new ServiceReply<List<int>>( ids.ToList() )
                : new ServiceReply<List<int>>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<List<int>>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<string?>> GetProductSearchQueryString( ProductSearchRequestDto requestDto )
    {
        await Task.Run( () => { } );
        return new ServiceReply<string?>( string.Empty );
    }
    public async Task<ServiceReply<bool>> UpdateProductsReviewData()
    {
        try
        {
            bool result = await _productRepository.UpdateReviewData();

            return result
                ? new ServiceReply<bool>( true )
                : new ServiceReply<bool>( ServiceErrorType.Conflict );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e, e.Message );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
    }

    static ProductEditModel MapEditToModel( ProductEditDto dto )
    {
        ProductEditModel m = new()
        {
            ProductId = dto.ProductId,
            VendorId = dto.VendorId,
            Title = dto.Title,
            ThumbnailUrl = dto.Thumbnail,
            Price = dto.Price,
            SalePrice = dto.SalePrice,
            ReleaseDate = dto.ReleaseDate,
            IsFeatured = dto.IsFeatured,
            Description = dto.Description,
            XmlSpecs = dto.XmlSpecsAsString,
            Categories = dto.Categories,
            SpecLookups = dto.LookupSpecs
        };

        string[] images = dto.ImagesAsString.Split( "," );
        m.Images = images.ToList();

        return m;
    }
    static ProductEditDto? MapDetailsToEdit( ProductDetailsModel? m )
    {
        if ( m?.Product is null )
            return null;

        ProductEditDto dto = new()
        {
            ProductId = m.Product.ProductId,
            VendorId = m.Product.VendorId,
            Title = m.Product.Title,
            Thumbnail = m.Product.Thumbnail,
            Price = m.Product.Price,
            SalePrice = m.Product.SalePrice,
            ReleaseDate = m.Product.ReleaseDate,
            IsFeatured = m.Product.IsFeatured,
            Description = m.Description?.Description ?? string.Empty,
            XmlSpecsAsString = m.XmlSpecs?.XmlSpecs ?? string.Empty
        };

        foreach ( ProductCategoryModel c in m.Categories )
            dto.Categories.Add( c.CategoryId );

        foreach ( ProductImageModel i in m.Images )
            dto.ImagesAsString = $"{dto.ImagesAsString},{i.ImageUrl}";

        foreach ( ProductSpecLookupModel l in m.SpecLookups )
        {
            if ( !dto.LookupSpecs.TryGetValue( l.SpecId, out List<int>? values ) )
            {
                values = new List<int>();
                dto.LookupSpecs.Add( l.SpecId, values );
            }

            values.Add( l.SpecValueId );
        }

        return dto;
    }
}