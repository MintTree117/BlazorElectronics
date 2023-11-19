using System.Collections.Specialized;
using System.Web;
using BlazorElectronics.Client.Services.Users;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Outbound.Users;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.UserViews;

public abstract class UserView : ComponentBase
{
    const string NOT_AUTHORIZED_MESSAGE = "You are not authorized to be here!";

    [Inject] 
    protected ILogger<UserView> Logger { get; init; } = default!;
    [Inject]
    protected NavigationManager NavManager { get; init; } = default!;
    [Inject]
    protected IUserServiceClient UserService { get; init; } = default!;

    protected string Message = string.Empty;
    protected bool IsLoaded = false;
    protected bool IsAuthorized = false;
    protected string? ReturnUrl = string.Empty;

    protected async Task AuthorizeUser()
    {
        ApiReply<UserSessionResponse?> response = await UserService.AuthorizeUser();
        IsAuthorized = response.Data is not null;
        if ( !IsAuthorized )
            Message = NOT_AUTHORIZED_MESSAGE;
    }
    protected void GetReturnUrl()
    {
        Uri uri = NavManager.ToAbsoluteUri( NavManager.Uri );
        NameValueCollection queryString = HttpUtility.ParseQueryString( uri.Query );
        ReturnUrl = queryString.Get( "returnUrl" );
    }
    protected async Task HandleRedirection()
    {
        await Task.Delay( 3000 );
        NavManager.NavigateTo( string.IsNullOrWhiteSpace( ReturnUrl ) ? "" : ReturnUrl );
    }
}