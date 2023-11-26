using BlazorElectronics.Server.Models.Products;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Products;

namespace BlazorElectronics.Server.Repositories.Products;

public interface IProductSearchRepository
{
    Task<IEnumerable<string>?> GetSearchSuggestions( string searchText, CategoryType categoryType, short categoryId );
    Task<string?> GetProductSearchQueryString( CategoryIdMap? categoryIdMap, ProductSearchRequest searchRequest );
    Task<IEnumerable<ProductSearchModel>?> GetProductSearch( CategoryIdMap? categoryIdMap, ProductSearchRequest searchRequest );
}