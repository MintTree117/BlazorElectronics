using BlazorElectronics.Shared.Mutual;

namespace BlazorElectronics.Client.Services.Cart;

public interface ICartService
{
    event Action OnChange;

    Task AddToCart( CartItem_DTO item );
    Task<Cart_DTO> GetItemsFromLocalStorage();
    Task<Cart_DTO?> GetItemsFromServer();
}