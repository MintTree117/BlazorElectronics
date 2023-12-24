namespace BlazorElectronics.Shared.Promos;

public sealed record PromoCodeDto
{
    public int PromoId { get; init; }
    public string PromoCode { get; init; } = string.Empty;
    public decimal Discount { get; init; }
    public bool IsActive { get; init; }
}