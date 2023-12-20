using BlazorElectronics.Client.Models;
using BlazorElectronics.Client.Services.Cart;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Cart;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.CartCheckout;

public partial class Cart : PageView
{
    [Inject] ICartServiceClient CartService { get; init; } = default!;
    CartModel _cart = new();

    protected override async Task OnInitializedAsync()
    {
        await CartService.UpdateCart();
        ServiceReply<CartModel?> cartReply = await CartService.UpdateCart();

        if ( !cartReply.Success || cartReply.Data is null )
        {
            SetViewMessage( false, cartReply.Message ?? "Failed to get cart products!" );
            return;
        }
        
        _cart = cartReply.Data;
        PageIsLoaded = true;
        StateHasChanged();
    }

    async Task ChangeQuantity( ChangeEventArgs args, CartProductDto product )
    {
        if ( !int.TryParse( args.Value?.ToString(), out int quantity ) )
            return;

        product.ItemQuantity = Math.Max( quantity, 1 );

        ServiceReply<bool> reply = await CartService.AddOrUpdateItem( product );

        if ( !reply.Success )
            InvokeAlert( AlertType.Danger, $"Failed to update item quantity! {reply.ErrorType} : {reply.Message}" );

        InvokeAlert( AlertType.Success, "Updated quantity." );
        StateHasChanged();
    }
    async Task RemoveItem( CartProductDto product )
    {
        ServiceReply<bool> reply = await CartService.RemoveItem( product.ProductId );

        if ( !reply.Success )
            InvokeAlert( AlertType.Danger, $"Failed to remove item! {reply.ErrorType} : {reply.Message}" );
    }
}