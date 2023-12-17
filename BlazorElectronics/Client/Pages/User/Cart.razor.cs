using BlazorElectronics.Client.Services.Users.Cart;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Cart;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.User;

public partial class Cart : PageView
{
    [Inject] ICartServiceClient CartService { get; init; } = default!;
    CartReplyDto? _cart;

    protected override async Task OnInitializedAsync()
    {
        ServiceReply<CartReplyDto?> reply = await CartService.GetCart();

        PageIsLoaded = true;
        
        if ( !reply.Success )
        {
            SetViewMessage( false, reply.Message ?? "Failed to get cart!" );
            return;
        }

        _cart = reply.Data;
    }
}