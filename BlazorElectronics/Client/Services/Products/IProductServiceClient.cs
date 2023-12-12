using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Products.Search;

namespace BlazorElectronics.Client.Services.Products;

public interface IProductServiceClient
{
    Task<ServiceReply<ProductSearchResponse?>> GetProductSearch( ProductSearchRequest request );
}