using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DtosInbound.Products;
using BlazorElectronics.Shared.DtosOutbound.Products;

namespace BlazorElectronics.Server.Services.Products;

public interface IProductService
{
    Task<ServiceResponse<string?>> GetProductSearchQueryString( ProductSearchRequest_DTO request );
    Task<ServiceResponse<Products_DTO?>> GetAllProducts();
    Task<ServiceResponse<ProductsFeatured_DTO?>> GetFeaturedProducts();
    Task<ServiceResponse<Products_DTO?>> GetTopDeals();
    Task<ServiceResponse<ProductSearchSuggestions_DTO?>> GetTextSearchSuggestions( string searchText );
    Task<ServiceResponse<ProductSearchResults_DTO?>> GetProductSearch( ProductSearchRequest_DTO searchRequestDto );
    Task<ServiceResponse<ProductDetails_DTO?>> GetProductDetails( int productId );
}