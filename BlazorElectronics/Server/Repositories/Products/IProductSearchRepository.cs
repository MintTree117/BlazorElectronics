using BlazorElectronics.Server.Models.Products;

namespace BlazorElectronics.Server.Repositories.Products;

public interface IProductSearchRepository : IDapperRepository<Product>
{
    Task<string?> GetProductSearchQueryString( ProductSearchRequest searchRequest );
    Task<IEnumerable<string>?> GetSearchSuggestions( string searchText );
    Task<ProductSearch?> GetProductSearch( ProductSearchRequest searchRequest );
}