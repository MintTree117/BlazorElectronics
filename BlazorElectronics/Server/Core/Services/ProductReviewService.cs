using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Server.Core.Models.Products;
using BlazorElectronics.Server.Data;
using BlazorElectronics.Server.Services;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.ProductReviews;
using BlazorElectronics.Shared.Products;

namespace BlazorElectronics.Server.Core.Services;

public sealed class ProductReviewService : ApiService, IProductReviewService
{
    readonly IProductReviewRepository _repository;

    public ProductReviewService( ILogger<ApiService> logger, IProductReviewRepository repository )
        : base( logger )
    {
        _repository = repository;
    }
    
    public async Task<ServiceReply<List<ProductReviewDto>?>> GetForProduct( int productId, int rows, int page )
    {
        try
        {
            IEnumerable<ProductReviewModel>? models = await _repository.GetForProduct( productId, rows, page );
            List<ProductReviewDto>? dtos = await MapModelsToDto( models );

            return dtos is not null
                ? new ServiceReply<List<ProductReviewDto>?>( dtos )
                : new ServiceReply<List<ProductReviewDto>?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<List<ProductReviewDto>?>( ServiceErrorType.ServerError );
        }
    }

    static async Task<List<ProductReviewDto>?> MapModelsToDto( IEnumerable<ProductReviewModel>? models )
    {
        if ( models is null )
            return null;

        return await Task.Run( () => ( 
                from m in models 
                where !string.IsNullOrWhiteSpace( m.Username ) 
                select new ProductReviewDto { Username = m.Username, Rating = m.Rating, Review = m.Review } )
            .ToList() );
    }
}