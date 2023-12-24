using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Features;

namespace BlazorElectronics.Client.Services.Features;

public interface IFeaturedDealsServiceClient
{
    Task<ServiceReply<List<FeatureDealDto>?>> GetFrontPageDeals();
    Task<ServiceReply<List<FeatureDealDto>?>> GetFeatureDeals( PaginationDto pagination );
}