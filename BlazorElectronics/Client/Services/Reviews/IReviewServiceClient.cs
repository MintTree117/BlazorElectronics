using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Reviews;

namespace BlazorElectronics.Client.Services.Reviews;

public interface IReviewServiceClient
{
    Task<ServiceReply<List<ProductReviewDto>?>> GetForProduct( GetProductReviewsDto dto );
}