using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Reviews;

namespace BlazorElectronics.Client.Services.Reviews;

public interface IReviewServiceClient
{
    Task<ServiceReply<ProductReviewsReplyDto?>> GetForProduct( ProductReviewsGetDto dto );
}