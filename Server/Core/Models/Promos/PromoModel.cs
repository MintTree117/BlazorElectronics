namespace BlazorElectronics.Server.Core.Models.Promos;

public sealed class PromoModel
{
    public int PromoId { get; set; }
    public string PromoCode { get; set; } = string.Empty;
    public float PromoDiscount { get; set; }
}