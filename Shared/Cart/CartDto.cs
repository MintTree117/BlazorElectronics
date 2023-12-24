using BlazorElectronics.Shared.Promos;

namespace BlazorElectronics.Shared.Cart;

public sealed class CartDto
{
    public List<CartProductDto> Products { get; set; } = new();
    public List<PromoCodeDto> PromoCodes { get; set; } = new();
}