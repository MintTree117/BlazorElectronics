using BlazorElectronics.Shared.Products;
using BlazorElectronics.Shared.Products.Search;

namespace BlazorElectronics.Server.Api.Interfaces;

public interface IProductServiceAdmin
{
    Task<ServiceReply<ProductEditDto?>> GetEdit( int id );
    Task<ServiceReply<int>> Add( ProductEditDto dto );
    Task<ServiceReply<bool>> Update( ProductEditDto dto );
    Task<ServiceReply<bool>> Remove( int id );
    Task<ServiceReply<List<int>>> GetAllIds();
    Task<ServiceReply<string?>> GetProductSearchQueryString( ProductSearchRequestDto requestDto );
    Task<ServiceReply<bool>> UpdateProductsReviewData();
}