using BlazorElectronics.Shared;
using BlazorElectronics.Shared.ProductReviews;

namespace BlazorElectronics.Client.Services.Products;

public interface IReviewServiceClient
{
    Task<ServiceReply<List<ProductReviewDto>?>> GetForProduct( GetProductReviewsDto dto );
}