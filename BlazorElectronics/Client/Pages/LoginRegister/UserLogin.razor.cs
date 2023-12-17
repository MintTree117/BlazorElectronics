using System.Collections.Specialized;
using System.Web;
using BlazorElectronics.Client.Services.Users;
using BlazorElectronics.Client.Services.Users.Cart;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Users;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.LoginRegister;

public partial class UserLogin : PageView
{
    [Inject] IUserServiceClient UserService { get; set; } = default!;
    [Inject] ICartServiceClient CartService { get; set; } = default!;
    readonly LoginRequestDto _loginRequestDto = new();

    string _returnUrl = string.Empty;

    protected override void OnInitialized()
    {
        Uri uri = NavManager.ToAbsoluteUri( NavManager.Uri );
        NameValueCollection queryString = HttpUtility.ParseQueryString( uri.Query );
        string? returnUrl = queryString.Get( "returnUrl" );

        _returnUrl = !string.IsNullOrWhiteSpace( returnUrl )
            ? returnUrl
            : "/profile";
    }
    async Task HandleLogin()
    {
        ServiceReply<SessionReplyDto?> loginReply = await UserService.Login( _loginRequestDto );

        if ( !loginReply.Success || loginReply.Data is null )
        {
            SetViewMessage( false, loginReply.Message ?? "Failed to login!" );
            return;
        }

        await CartService.UpdateCart();
        NavManager.NavigateTo( _returnUrl );
    }
}