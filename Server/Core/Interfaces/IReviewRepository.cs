using BlazorElectronics.Server.Core.Models.Reviews;
using BlazorElectronics.Shared.Reviews;

namespace BlazorElectronics.Server.Core.Interfaces;

public interface IReviewRepository
{
    Task<IEnumerable<ReviewModel>?> GetForProduct( ProductReviewsGetDto dto );
    Task<IEnumerable<ReviewModel>?> GetForUser( int userId, int rows, int page );
    Task<ReviewModel?> GetEdit( int reviewId );
    Task<int> Insert( ReviewModel reviewModel );
    Task<bool> Update( ReviewModel reviewModel );
    Task<bool> Delete( int reviewId );
}