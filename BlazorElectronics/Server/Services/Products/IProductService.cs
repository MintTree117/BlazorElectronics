using BlazorElectronics.Server.Dtos.Specs;
using BlazorElectronics.Shared.DtosOutbound.Products;
using BlazorElectronics.Shared.Inbound.Products;
using BlazorElectronics.Shared.Mutual;

namespace BlazorElectronics.Server.Services.Products;

public interface IProductService
{
    Task<ApiReply<string?>> GetProductSearchQueryString( ProductSearchRequest request );
    Task<ApiReply<Products_DTO?>> GetAllProducts();
    Task<ApiReply<ProductSearchSuggestions_DTO?>> GetProductSuggestions( ProductSuggestionRequest request );
    Task<ApiReply<ProductSearchResults_DTO?>> GetProductSearch( CategoryIdMap? idMap, ProductSearchRequest? request, CachedSpecData specMeta );
    Task<ApiReply<ProductDetails_DTO?>> GetProductDetails( int productId );
}