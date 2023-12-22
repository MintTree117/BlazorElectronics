namespace BlazorElectronics.Shared.Promos;

public sealed class PromoCodeDto
{
    public int PromoId { get; set; }
    public string PromoCode { get; set; } = string.Empty;
    public double Discount { get; set; }
}