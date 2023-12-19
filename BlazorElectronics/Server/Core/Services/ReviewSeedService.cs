using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Server.Core.Models.Reviews;
using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Server.Core.Services;

public sealed class ReviewSeedService : ApiService, IReviewSeedService
{
    readonly IReviewRepository _repository;
    readonly Random _random = new();
    
    public ReviewSeedService( ILogger<ApiService> logger, IReviewRepository repository )
        : base( logger )
    {
        _repository = repository;
    }

    public async Task<ServiceReply<bool>> SeedReviews( int amount, List<int> productIds, List<int> userIds )
    {
        List<ReviewModel> reviews = await GetRandomReviews( amount, productIds, userIds );

        foreach ( ReviewModel r in reviews )
        {
            try
            {
                int id = await _repository.Insert( r );

                if ( id <= 0 )
                    Logger.LogError( "Insertion failed" );
            }
            catch ( RepositoryException e )
            {
                Logger.LogError( e.Message, e );
                return new ServiceReply<bool>( ServiceErrorType.ServerError );
            }
        }

        return new ServiceReply<bool>( true );
    }
    async Task<List<ReviewModel>> GetRandomReviews( int amount, IReadOnlyList<int> productIds, IReadOnlyList<int> userIds )
    {
        return await Task.Run( () =>
        {
            List<ReviewModel> reviews = new();

            foreach ( int id in productIds )
            {
                int productAmount = GetRandomInt( 1, amount );

                for ( int j = 0; j < productAmount; j++ )
                {
                    reviews.Add( new ReviewModel
                    {
                        ProductId = id,
                        UserId = userIds[ GetRandomInt( 0, userIds.Count - 1 ) ],
                        Review = GetRandomItem( SeedData.PRODUCT_REVIEWS[ GetRandomInt( 0, SeedData.PRODUCT_REVIEWS.Length - 1 ) ] ),
                        Rating = GetRandomInt( 1, 5 )
                    } );
                }
            }

            return reviews;
        } );
    }

    int GetRandomInt( int min, int max )
    {
        if ( min >= max )
            throw new ArgumentOutOfRangeException( nameof( min ), "min must be less than max" );

        return _random.Next( min, max );
    }
    string GetRandomItem( IReadOnlyList<string> list )
    {
        int index = GetRandomInt( 0, list.Count - 1 );
        return list[ index ];
    }
}