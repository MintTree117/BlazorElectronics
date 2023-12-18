using System.Collections.Specialized;
using System.Web;
using BlazorElectronics.Client.Services.Users;
using BlazorElectronics.Client.Services.Users.Cart;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Users;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.Login;

public partial class UserLogin : PageView
{
    [Inject] IUserServiceClient UserService { get; set; } = default!;
    [Inject] ICartServiceClient CartService { get; set; } = default!;
    readonly LoginRequestDto _loginDto = new();
    readonly RegisterRequestDto _registerDto = new();

    string _returnUrl = string.Empty;
    bool _registered = false;

    protected override void OnInitialized()
    {
        Uri uri = NavManager.ToAbsoluteUri( NavManager.Uri );
        NameValueCollection queryString = HttpUtility.ParseQueryString( uri.Query );
        string? returnUrl = queryString.Get( "returnUrl" );

        _returnUrl = !string.IsNullOrWhiteSpace( returnUrl )
            ? returnUrl
            : "/profile";

        PageIsLoaded = true;
    }
    async Task HandleLogin()
    {
        ServiceReply<SessionReplyDto?> loginReply = await UserService.Login( _loginDto );

        if ( !loginReply.Success || loginReply.Data is null )
        {
            SetViewMessage( false, loginReply.Message ?? "Failed to login!" );
            return;
        }

        await CartService.UpdateCart();
        NavManager.NavigateTo( _returnUrl );
    }
    async Task HandleRegistration()
    {
        ServiceReply<bool> reply = await UserService.Register( _registerDto );

        if ( !reply.Success )
        {
            InvokeAlert( AlertType.Danger, reply.Message ?? "Failed to register!" );
            return;
        }

        _registered = true;
        InvokeAlert( AlertType.Success, "Successfully registered account!" );
        StateHasChanged();
    }
}