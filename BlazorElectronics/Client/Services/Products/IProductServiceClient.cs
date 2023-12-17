using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Products;
using BlazorElectronics.Shared.Products.Search;

namespace BlazorElectronics.Client.Services.Products;

public interface IProductServiceClient
{
    Task<ServiceReply<List<string>?>> GetProductSearchSuggestions( string searchText );
    Task<ServiceReply<ProductSearchReplyDto?>> GetProductSearch( ProductSearchRequestDto requestDto );
    Task<ServiceReply<ProductDto?>> GetProductDetails( int productId );
}