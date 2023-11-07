using BlazorElectronics.Server.Dtos.Specs;
using BlazorElectronics.Shared.DtosOutbound.Products;
using BlazorElectronics.Shared.Inbound.Products;
using BlazorElectronics.Shared.Mutual;

namespace BlazorElectronics.Server.Services.Products;

public interface IProductService
{
    Task<Reply<string?>> GetProductSearchQueryString( ProductSearchRequest request );
    Task<Reply<Products_DTO?>> GetAllProducts();
    Task<Reply<ProductSearchSuggestions_DTO?>> GetProductSuggestions( ProductSuggestionRequest request );
    Task<Reply<ProductSearchResults_DTO?>> GetProductSearch( CategoryIdMap? idMap, ProductSearchRequest? request, SpecLookupTableMetaDto specMeta );
    Task<Reply<ProductDetails_DTO?>> GetProductDetails( int productId );
}