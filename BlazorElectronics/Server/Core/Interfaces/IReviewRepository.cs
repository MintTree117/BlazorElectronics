using BlazorElectronics.Server.Core.Models.Products;

namespace BlazorElectronics.Server.Core.Interfaces;

public interface IReviewRepository
{
    Task<IEnumerable<ProductReviewModel>?> GetForProduct( int productId, int rows, int page );
    Task<IEnumerable<ProductReviewModel>?> GetForUser( int userId, int rows, int page );
    Task<ProductReviewModel?> GetEdit( int reviewId );
    Task<int> Insert( ProductReviewModel reviewModel );
    Task<bool> Update( ProductReviewModel reviewModel );
    Task<bool> Delete( int reviewId );
}