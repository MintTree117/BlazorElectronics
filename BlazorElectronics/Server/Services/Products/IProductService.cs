using BlazorElectronics.Server.Dtos.Specs;
using BlazorElectronics.Shared.DtosOutbound.Products;
using BlazorElectronics.Shared.Inbound.Products;
using BlazorElectronics.Shared.Mutual;

namespace BlazorElectronics.Server.Services.Products;

public interface IProductService
{
    Task<ApiReply<string?>> GetProductSearchQueryString( ProductSearchRequest request );
    Task<ApiReply<ProductSearchSuggestions_DTO?>> GetProductSuggestions( ProductSuggestionRequest request );
    Task<ApiReply<ProductSearchResponse?>> GetProductSearch( CategoryIdMap? idMap, ProductSearchRequest? request, CachedSpecData specData );
    Task<ApiReply<ProductDetails_DTO?>> GetProductDetails( int productId );
}