using BlazorElectronics.Server.Models.Products;

namespace BlazorElectronics.Server.Repositories.Products;

public interface IProductReviewRepository
{
    Task<IEnumerable<ProductReview>?> GetForProduct( int productId, int rows, int offset );
    Task<IEnumerable<ProductReview>?> GetForUser( int userId, int rows, int offset );
    Task<ProductReview?> GetEdit( int reviewId );
    Task<int> Insert( ProductReview review );
    Task<bool> Update( ProductReview review );
    Task<bool> Delete( int reviewId );
}