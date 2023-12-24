using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Server.Core.Models.Reviews;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.Reviews;

namespace BlazorElectronics.Server.Core.Services;

public sealed class ReviewService : ApiService, IReviewService
{
    readonly IReviewRepository _repository;

    public ReviewService( ILogger<ApiService> logger, IReviewRepository repository )
        : base( logger )
    {
        _repository = repository;
    }
    
    public async Task<ServiceReply<ProductReviewsReplyDto?>> GetForProduct( ProductReviewsGetDto dto )
    {
        try
        {
            IEnumerable<ReviewModel>? models = await _repository.GetForProduct( dto );
            ProductReviewsReplyDto? dtos = await MapModelsToDto( models );

            return dtos is not null
                ? new ServiceReply<ProductReviewsReplyDto?>( dtos )
                : new ServiceReply<ProductReviewsReplyDto?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<ProductReviewsReplyDto?>( ServiceErrorType.ServerError );
        }
    }
    static async Task<ProductReviewsReplyDto?> MapModelsToDto( IEnumerable<ReviewModel>? models )
    {
        if ( models is null )
            return null;

        return await Task.Run( () =>
        {
            ProductReviewsReplyDto dto = new();

            foreach ( ReviewModel m in models )
            {
                dto.TotalMatches = m.TotalCount;
                dto.Reviews.Add( new ProductReviewDto
                {
                    Username = m.Username,
                    Rating = m.Rating,
                    Review = m.Review,
                    Date = m.Date
                } );
            }
            
            return dto;
        } );
    }
}