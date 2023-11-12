namespace BlazorElectronics.Shared.Inbound.Admin.Features;

public abstract class AddOrDeleteFeaturedDealRequest : AdminRequest
{
    public int ProductId { get; set; }
    public int VariantId { get; set; }
}