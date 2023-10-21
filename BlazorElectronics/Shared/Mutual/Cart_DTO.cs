namespace BlazorElectronics.Shared.Mutual;

public sealed class Cart_DTO
{
    public List<CartItem_DTO> Items { get; set; } = new();

    public decimal GetTotalPrice()
    {
        return Items.Sum( item => item.GetRealPrice() * item.Quantity );
    }
}