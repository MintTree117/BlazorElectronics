namespace BlazorElectronics.Shared.Admin.Products;

public sealed class RemoveProductDto
{
    public RemoveProductDto( int productId )
    {
        ProductId = productId;
    }
    
    public int ProductId { get; }
}