using BlazorElectronics.Shared.Orders;

namespace BlazorElectronics.Server.Core.Models.Orders;

public sealed record OrderDetailsModel
{
    public OrderDetailsMetaModel? Meta { get; set; }
    public IEnumerable<OrderDetailsProductDto>? Items { get; set; }
}