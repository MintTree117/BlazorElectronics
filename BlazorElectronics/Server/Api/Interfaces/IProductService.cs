using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Outbound.Products;
using BlazorElectronics.Shared.Products.Search;

namespace BlazorElectronics.Server.Api.Interfaces;

public interface IProductService
{
    Task<ServiceReply<string?>> GetProductSearchQueryString( ProductSearchRequest request );
    Task<ServiceReply<List<string>?>> GetProductSuggestions( string searchText );
    Task<ServiceReply<ProductSearchResponse?>> GetProductSearch( ProductSearchRequest request );
    Task<ServiceReply<ProductDetailsResponse?>> GetProductDetails( int productId, CategoryData categoryData );
}