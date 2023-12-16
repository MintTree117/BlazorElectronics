using BlazorElectronics.Shared.ProductReviews;
using BlazorElectronics.Shared.Products;

namespace BlazorElectronics.Server.Api.Interfaces;

public interface IReviewService
{
    Task<ServiceReply<List<ProductReviewDto>?>> GetForProduct( int productId, int rows, int page );
}