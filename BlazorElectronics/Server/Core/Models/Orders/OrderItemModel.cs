namespace BlazorElectronics.Server.Core.Models.Orders;

public sealed record OrderItemModel
{
    public int ProductId { get; init; }
    public int Quantity { get; init; }
    public decimal TotalPrice { get; init; }
}