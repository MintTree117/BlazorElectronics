using BlazorElectronics.Client.Services.Cart;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Cart;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.User;

public partial class Cart : PageView
{
    [Inject] ICartServiceClient CartService { get; init; } = default!;
    CartReplyDto _cart = new();

    protected override async Task OnInitializedAsync()
    {
        ServiceReply<CartReplyDto?> reply = await CartService.GetCart();

        PageIsLoaded = true;
        
        if ( !reply.Success || reply.Data is null )
        {
            SetViewMessage( false, reply.Message ?? "Failed to get cart!" );
            return;
        }

        _cart = reply.Data;
    }

    async Task ChangeQuantity( ChangeEventArgs args, CartProductDto product )
    {
        if ( !int.TryParse( args.Value?.ToString(), out int quantity ) )
            return;
        
        CartItemDto dto = new()
        {
            ProductId = product.ProductId,
            Quantity = Math.Max( quantity, 1 )
        };

        ServiceReply<bool> reply = await CartService.UpdateCartQuantity( dto );

        if ( !reply.Success )
            InvokeAlert( AlertType.Danger, $"Failed to update item quantity! {reply.ErrorType} : {reply.Message}" );
    }
    async Task RemoveItem( CartProductDto product )
    {
        ServiceReply<bool> reply = await CartService.RemoveFromCart( product.ProductId );

        if ( !reply.Success )
            InvokeAlert( AlertType.Danger, $"Failed to remove item! {reply.ErrorType} : {reply.Message}" );
    }
}