using BlazorElectronics.Shared.Promos;

namespace BlazorElectronics.Server.Core.Models.Orders;

public sealed record OrderModel
{
    public int UserId { get; init; }
    public DateTime OrderDate { get; init; } = DateTime.Now;
    public decimal TotalPrice { get; init; }
    public List<OrderItemModel> OrderItems { get; init; } = new();
    public List<PromoCodeDto> PromoCodes { get; init; } = new();
}