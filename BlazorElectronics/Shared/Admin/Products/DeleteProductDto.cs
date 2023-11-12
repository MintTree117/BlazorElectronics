namespace BlazorElectronics.Shared.Admin.Products;

public sealed class DeleteProductDto
{
    public DeleteProductDto( int productId )
    {
        ProductId = productId;
    }
    
    public int ProductId { get; }
}