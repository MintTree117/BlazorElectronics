using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DataTransferObjects.Products;

namespace BlazorElectronics.Server.Services.Products;

public interface IProductService
{
    Task<ServiceResponse<string?>> TestGetQueryString( ProductSearchRequest_DTO request );
    Task<ServiceResponse<Products_DTO?>> GetProducts();
    Task<ServiceResponse<ProductsFeatured_DTO?>> GetFeaturedProducts();
    Task<ServiceResponse<ProductSearchSuggestions_DTO?>> GetProductSearchSuggestions( string searchText );
    Task<ServiceResponse<ProductSearchResults_DTO?>> SearchProducts( ProductSearchRequest_DTO? searchRequestDto );
    Task<ServiceResponse<ProductDetails_DTO?>> GetProductDetails( int productId );
}