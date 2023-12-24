namespace BlazorElectronics.Shared.Orders;

public sealed record OrderDetailsDto
{
    public DateTime OrderDate { get; init; }
    public decimal TotalPrice { get; init; }
    public List<OrderDetailsProductDto> Products { get; init; } = new();
}