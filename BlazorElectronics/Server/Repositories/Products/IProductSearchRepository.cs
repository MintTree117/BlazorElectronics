using BlazorElectronics.Server.Dtos.Specs;
using BlazorElectronics.Server.Models.Products;
using BlazorElectronics.Shared.Inbound.Products;
using BlazorElectronics.Shared.Mutual;

namespace BlazorElectronics.Server.Repositories.Products;

public interface IProductSearchRepository
{
    Task<IEnumerable<string>?> GetSearchSuggestions( string searchText, int categoryTier, short categoryId );
    Task<string?> GetProductSearchQueryString( CategoryIdMap? categoryIdMap, ProductSearchRequest searchRequest, CachedSpecData specData );
    Task<ProductSearch?> GetProductSearch( CategoryIdMap? categoryIdMap, ProductSearchRequest searchRequest, CachedSpecData specData );
}