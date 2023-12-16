using BlazorElectronics.Shared.Products;
using BlazorElectronics.Shared.Products.Search;

namespace BlazorElectronics.Server.Api.Interfaces;

public interface IProductService
{
    Task<ServiceReply<List<int>>> GetAllIds();
    Task<ServiceReply<string?>> GetProductSearchQueryString( ProductSearchRequest request );
    Task<ServiceReply<List<string>?>> GetProductSuggestions( string searchText );
    Task<ServiceReply<ProductSearchResponse?>> GetProductSearch( ProductSearchRequest request );
    Task<ServiceReply<ProductDto?>> GetProductDetails( int productId );
}