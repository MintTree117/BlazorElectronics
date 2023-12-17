using BlazorElectronics.Shared.Products;
using BlazorElectronics.Shared.Reviews;

namespace BlazorElectronics.Server.Api.Interfaces;

public interface IReviewService
{
    Task<ServiceReply<List<ProductReviewDto>?>> GetForProduct( GetProductReviewsDto dto );
}