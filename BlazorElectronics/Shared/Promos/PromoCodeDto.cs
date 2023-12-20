namespace BlazorElectronics.Shared.Promos;

public sealed class PromoCodeDto
{
    public string Code { get; set; } = string.Empty;
    public double Discount { get; set; }
}