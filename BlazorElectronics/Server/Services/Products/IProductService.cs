using BlazorElectronics.Shared.DtosOutbound.Products;
using BlazorElectronics.Shared.Inbound.Products;
using BlazorElectronics.Shared.Mutual;

namespace BlazorElectronics.Server.Services.Products;

public interface IProductService
{
    Task<ApiReply<string?>> GetProductSearchQueryString( ProductSearchRequest request );
    Task<ApiReply<ProductSuggestionsResponse?>> GetProductSuggestions( ProductSuggestionRequest request );
    Task<ApiReply<ProductSearchResponse?>> GetProductSearch( CategoryIdMap? idMap, ProductSearchRequest? request );
    Task<ApiReply<ProductDetailsResponse?>> GetProductDetails( int productId );
}