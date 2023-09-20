using BlazorElectronics.Shared.DataTransferObjects.Products;

namespace BlazorElectronics.Client.Services.Products;

public interface IProductServiceClient
{
    List<Product_DTO>? Products { get; set; }
    Task GetProducts();
}