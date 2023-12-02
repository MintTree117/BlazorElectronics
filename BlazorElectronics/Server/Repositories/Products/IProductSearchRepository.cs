using BlazorElectronics.Server.Models.Products;
using BlazorElectronics.Shared.Products.Search;

namespace BlazorElectronics.Server.Repositories.Products;

public interface IProductSearchRepository
{
    Task<IEnumerable<string>?> GetSearchSuggestions( string searchText );
    Task<IEnumerable<ProductSearchModel>?> GetProductSearch( int? categoryId, ProductSearchRequest searchRequest );
}