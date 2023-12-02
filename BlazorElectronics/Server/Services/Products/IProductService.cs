using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Outbound.Products;
using BlazorElectronics.Shared.Products;
using BlazorElectronics.Shared.Products.Search;

namespace BlazorElectronics.Server.Services.Products;

public interface IProductService
{
    Task<ServiceReply<string?>> GetProductSearchQueryString( ProductSearchRequest request );
    Task<ServiceReply<ProductSuggestionsResponse?>> GetProductSuggestions( ProductSuggestionRequest request );
    Task<ServiceReply<ProductSearchResponse?>> GetProductSearch( int? categoryId, ProductSearchRequest? request );
    Task<ServiceReply<ProductDetailsResponse?>> GetProductDetails( int productId, CategoriesResponse categoriesResponse );
}