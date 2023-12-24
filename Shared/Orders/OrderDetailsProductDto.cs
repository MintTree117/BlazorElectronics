namespace BlazorElectronics.Shared.Orders;

public sealed record OrderDetailsProductDto
{
    public int ProductId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal TotalPrice { get; init; }
}