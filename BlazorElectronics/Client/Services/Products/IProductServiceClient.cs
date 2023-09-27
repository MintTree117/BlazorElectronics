using BlazorElectronics.Shared.DataTransferObjects.Products;

namespace BlazorElectronics.Client.Services.Products;

public interface IProductServiceClient
{
    event Action ProductsChanged;

    int PageNumber { get; set; }
    int ProductCount { get; set; }
    List<Product_DTO>? Products { get; set; }
    
    ProductDetails_DTO? ProductDetails { get; set; }

    Task GetProductsTEST( string query );
    Task GetProducts( ProductSearchFilters_DTO? filters );
    Task GetProductDetails( int productId );
}