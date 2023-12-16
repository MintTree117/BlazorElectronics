namespace BlazorElectronics.Server.Api.Interfaces;

public interface IReviewSeedService
{
    Task<ServiceReply<bool>> SeedReviews( int amount, List<int> productIds, List<int> userIds );
}