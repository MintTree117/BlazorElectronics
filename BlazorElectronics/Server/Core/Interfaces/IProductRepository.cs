using BlazorElectronics.Server.Core.Models.Products;

namespace BlazorElectronics.Server.Core.Interfaces;

public interface IProductRepository
{
    Task<ProductModel?> Get( int productId );
    Task<int> Insert( ProductModel model );
    Task<bool> Update( ProductModel model );
    Task<bool> Delete( int productId );

    Task<bool> UpdateRatings();
    Task<bool> UpdateReviewCount();
}