using BlazorElectronics.Server.Models.Products;

namespace BlazorElectronics.Server.Repositories.Products;

public interface IProductRepository
{
    Task<string> TEST_GET_QUERY_STRING( ValidatedSearchFilters searchFilters );
    Task<(IEnumerable<Product>?, int)?> GetAllProducts();
    Task<(IEnumerable<Product>?, int)?> SearchProducts( ValidatedSearchFilters searchFilters );
    Task<ProductDetails?> GetProductDetails( int productId );
}