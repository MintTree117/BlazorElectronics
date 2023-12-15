using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Products.Search;

namespace BlazorElectronics.Client.Services.Products;

public interface IProductServiceClient
{
    Task<ServiceReply<List<string>?>> GetProductSearchSuggestions( string searchText );
    Task<ServiceReply<ProductSearchResponse?>> GetProductSearch( ProductSearchRequest request );
}