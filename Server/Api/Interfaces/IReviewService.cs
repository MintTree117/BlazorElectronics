using BlazorElectronics.Shared.Reviews;

namespace BlazorElectronics.Server.Api.Interfaces;

public interface IReviewService
{
    Task<ServiceReply<ProductReviewsReplyDto?>> GetForProduct( ProductReviewsGetDto dto );
}