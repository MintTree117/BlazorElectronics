using BlazorElectronics.Shared.Products;
using BlazorElectronics.Shared.Products.Search;

namespace BlazorElectronics.Server.Api.Interfaces;

public interface IProductServiceCustomer
{
    Task<ServiceReply<List<string>?>> GetSuggestions( string searchText );
    Task<ServiceReply<ProductSearchReplyDto?>> GetSearch( ProductSearchRequestDto requestDto );
    Task<ServiceReply<ProductDto?>> GetDetails( int productId );
}