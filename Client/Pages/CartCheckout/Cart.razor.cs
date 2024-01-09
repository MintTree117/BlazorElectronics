using BlazorElectronics.Client.Models;
using BlazorElectronics.Client.Services.Cart;
using BlazorElectronics.Client.Services.Checkout;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Cart;
using BlazorElectronics.Shared.Promos;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.CartCheckout;

public partial class Cart : PageView
{
    [Inject] ICartServiceClient CartService { get; init; } = default!;
    [Inject] ICheckoutServiceClient CheckoutService { get; init; } = default!;
    public CartModel CartModel = new();

    CartSummary _summary = default!;

    protected override async Task OnInitializedAsync()
    {
        ServiceReply<CartModel?> cartReply = await CartService.UpdateCart();

        if ( !cartReply.Success || cartReply.Payload is null )
        {
            SetViewMessage( false, cartReply.Message ?? "Failed to get cart products!" );
            return;
        }
        
        CartModel = cartReply.Payload;
        PageIsLoaded = true;
        StateHasChanged();
    }

    public async Task AddPromoCode( string code )
    {
        ServiceReply<PromoCodeDto?> reply = await CartService.AddPromoCode( code );

        if ( reply.Success )
        {
            _summary.OnAddSuccess();
            return;
        }
    }
    public async Task RemovePromoCode( int id )
    {
        ServiceReply<bool> reply = await CartService.RemovePromoCode( id );
    }
    
    async Task ChangeQuantity( ChangeEventArgs args, CartProductDto product )
    {
        if ( !int.TryParse( args.Value?.ToString(), out int quantity ) )
            return;

        product.Quantity = Math.Max( quantity, 1 );

        ServiceReply<CartModel?> reply = await CartService.AddOrUpdateItem( product );

        if ( !reply.Success || reply.Payload is null )
        {
            InvokeAlert( AlertType.Danger, $"Failed to update item! {reply.ErrorType} : {reply.Message}" );
            return;
        }

        CartModel = reply.Payload;
        InvokeAlert( AlertType.Success, "Updated quantity." );
        StateHasChanged();
    }
    async Task RemoveItem( CartProductDto product )
    {
        ServiceReply<CartModel?> reply = await CartService.RemoveItem( product.ProductId );

        if ( !reply.Success || reply.Payload is null )
        {
            InvokeAlert( AlertType.Danger, $"Failed to remove item! {reply.ErrorType} : {reply.Message}" );  
            return;
        }

        CartModel = reply.Payload;
        InvokeAlert( AlertType.Success, "Removed item." );
        StateHasChanged();
    }
    async Task EmptyCart()
    {
        await CartService.ClearCart();
        CartModel = new CartModel();
    }

    async Task PlaceOrder()
    {
        ServiceReply<string?> orderReply = await CheckoutService.PlaceOrder();

        if ( !orderReply.Success || orderReply.Payload is null )
        {
            InvokeAlert( AlertType.Danger, $"Failed to place order! {orderReply.ErrorType} : {orderReply.Message}" );
            return;
        }

        string url = orderReply.Payload;
        NavManager.NavigateTo( url );
    }
}