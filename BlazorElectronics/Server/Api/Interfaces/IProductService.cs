using BlazorElectronics.Shared.Products;
using BlazorElectronics.Shared.Products.Search;

namespace BlazorElectronics.Server.Api.Interfaces;

public interface IProductService
{
    Task<ServiceReply<List<int>>> GetAllIds();
    Task<ServiceReply<string?>> GetProductSearchQueryString( ProductSearchRequestDto requestDto );
    Task<ServiceReply<List<string>?>> GetProductSuggestions( string searchText );
    Task<ServiceReply<ProductSearchReplyDto?>> GetProductSearch( ProductSearchRequestDto requestDto );
    Task<ServiceReply<ProductDto?>> GetProductDetails( int productId );
    Task<ServiceReply<bool>> UpdateProductsReviewData();
}