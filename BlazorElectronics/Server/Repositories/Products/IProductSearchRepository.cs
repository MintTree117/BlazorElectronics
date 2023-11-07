using BlazorElectronics.Server.Dtos.Specs;
using BlazorElectronics.Server.Models.Products;
using BlazorElectronics.Server.Models.Specs;
using BlazorElectronics.Shared.Inbound.Products;
using BlazorElectronics.Shared.Mutual;

namespace BlazorElectronics.Server.Repositories.Products;

public interface IProductSearchRepository
{
    Task<string?> GetProductSearchQueryString( ProductSearchRequest searchRequest );
    Task<IEnumerable<string>?> GetSearchSuggestions( string searchText, int categoryTier, short categoryId );
    Task<ProductSearch?> GetProductSearch( CategoryIdMap categoryIdMap, ProductSearchRequest searchRequest, SpecLookupTableMetaDto specTableMeta );
}