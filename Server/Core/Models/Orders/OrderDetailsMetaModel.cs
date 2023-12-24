namespace BlazorElectronics.Server.Core.Models.Orders;

public sealed record OrderDetailsMetaModel
{
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
}