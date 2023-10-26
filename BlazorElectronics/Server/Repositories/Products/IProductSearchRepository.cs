using BlazorElectronics.Server.Models.Products;

namespace BlazorElectronics.Server.Repositories.Products;

public interface IProductSearchRepository
{
    Task<string?> GetProductSearchQueryString( ProductSearchRequest searchRequest );
    Task<Product?> GetProducyById( int id );
    Task<IEnumerable<Product>?> GetAllProducts();
    Task<IEnumerable<string>?> GetSearchSuggestions( string searchText );
    Task<ProductSearch?> GetProductSearch( ProductSearchRequest searchRequest );
}