using BlazorElectronics.Server.Core.Models.Products;

namespace BlazorElectronics.Server.Core.Interfaces;

public interface IProductRepository
{
    Task<ProductModel?> Get( int productId );
    Task<int> Insert( ProductEditModel editModel );
    Task<bool> Update( ProductEditModel editModel );
    Task<bool> Delete( int productId );

    Task<bool> UpdateRatings();
    Task<bool> UpdateReviewCount();
}