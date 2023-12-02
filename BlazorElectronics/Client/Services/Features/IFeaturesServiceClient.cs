using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Features;

namespace BlazorElectronics.Client.Services.Features;

public interface IFeaturesServiceClient
{
    Task<ServiceReply<List<Feature>?>> GetFeatures();
    Task<ServiceReply<List<FeaturedDeal>?>> GetFeaturedDeals();
}