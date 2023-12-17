namespace BlazorElectronics.Shared.Cart;

public sealed class CartRequestDto
{
    public List<CartItemDto> Items { get; init; } = new();
}