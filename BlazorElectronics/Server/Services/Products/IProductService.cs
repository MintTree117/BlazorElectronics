using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Outbound.Products;
using BlazorElectronics.Shared.Products;
using BlazorElectronics.Shared.Products.Search;

namespace BlazorElectronics.Server.Services.Products;

public interface IProductService
{
    Task<ApiReply<string?>> GetProductSearchQueryString( ProductSearchRequest request );
    Task<ApiReply<ProductSuggestionsResponse?>> GetProductSuggestions( ProductSuggestionRequest request );
    Task<ApiReply<ProductSearchResponse?>> GetProductSearch( CategoryIdMap? idMap, ProductSearchRequest? request );
    Task<ApiReply<ProductDetailsResponse?>> GetProductDetails( int productId, CategoriesResponse categoriesResponse );
}