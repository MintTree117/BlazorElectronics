using BlazorElectronics.Shared.DataTransferObjects.Products;

namespace BlazorElectronics.Client.Services.Products;

public interface IProductServiceClient
{
    ProductDetails_DTO? ProductDetails { get; set; }
    List<Product_DTO>? Products { get; set; }
    Task GetProducts();
    Task GetProductDetails( int productId );
}