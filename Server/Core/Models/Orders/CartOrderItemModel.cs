namespace BlazorElectronics.Server.Core.Models.Orders;

public sealed record CartOrderItemModel
{
    public int ProductId { get; set; }
    public decimal Price { get; set; }
    public decimal? SalePrice { get; set; }
    public int Quantity { get; set; }
}