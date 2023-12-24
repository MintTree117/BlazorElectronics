using BlazorElectronics.Shared.Promos;

namespace BlazorElectronics.Server.Core.Models.Orders;

public sealed record CartOrderModel
{
    public IEnumerable<CartOrderItemModel>? Items { get; set; }
    public IEnumerable<PromoCodeDto>? Promos { get; set; }
}