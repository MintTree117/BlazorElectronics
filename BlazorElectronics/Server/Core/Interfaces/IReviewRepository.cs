using BlazorElectronics.Server.Core.Models.Products;
using BlazorElectronics.Shared.Reviews;

namespace BlazorElectronics.Server.Core.Interfaces;

public interface IReviewRepository
{
    Task<IEnumerable<ProductReviewModel>?> GetForProduct( GetProductReviewsDto dto );
    Task<IEnumerable<ProductReviewModel>?> GetForUser( int userId, int rows, int page );
    Task<ProductReviewModel?> GetEdit( int reviewId );
    Task<int> Insert( ProductReviewModel reviewModel );
    Task<bool> Update( ProductReviewModel reviewModel );
    Task<bool> Delete( int reviewId );
}