namespace BlazorElectronics.Shared.Products;

public sealed class ProductKeysDto
{
    public int ProductId { get; set; }
    public List<string> Keys { get; set; } = new();
}