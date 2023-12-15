using BlazorElectronics.Server.Core.Models.Products;
using BlazorElectronics.Shared.Products.Search;

namespace BlazorElectronics.Server.Core.Interfaces;

public interface IProductSearchRepository
{
    Task<string?> GetProductSearchQuery( ProductSearchRequest searchRequest );
    Task<IEnumerable<string>?> GetSearchSuggestions( string searchText );
    Task<IEnumerable<ProductSearchModel>?> GetProductSearch( ProductSearchRequest searchRequest );
}