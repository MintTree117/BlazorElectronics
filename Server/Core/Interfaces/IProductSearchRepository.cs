using BlazorElectronics.Server.Core.Models.Products;
using BlazorElectronics.Shared.Products.Search;

namespace BlazorElectronics.Server.Core.Interfaces;

public interface IProductSearchRepository
{
    Task<string?> GetSearchQuery( ProductSearchRequestDto searchRequestDto );
    Task<IEnumerable<string>?> GetSearchSuggestions( string searchText );
    Task<IEnumerable<ProductSearchModel>?> GetProductSearch( ProductSearchRequestDto searchRequestDto );
}