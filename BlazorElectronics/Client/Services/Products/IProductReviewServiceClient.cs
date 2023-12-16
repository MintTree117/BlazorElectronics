using BlazorElectronics.Shared;
using BlazorElectronics.Shared.ProductReviews;

namespace BlazorElectronics.Client.Services.Products;

public interface IProductReviewServiceClient
{
    Task<ServiceReply<List<ProductReviewDto>?>> GetForProduct( GetProductReviewsDto dto );
}