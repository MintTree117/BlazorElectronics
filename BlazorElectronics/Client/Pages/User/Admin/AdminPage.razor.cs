using BlazorElectronics.Client.Services.Users.Admin;
using BlazorElectronics.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.User.Admin;

public abstract class AdminPage : UserPage
{
    [Inject] IAdminServiceClient AdminService { get; init; } = default!;

    protected override async Task OnInitializedAsync()
    {
        GetReturnUrl();
        await AuthorizeAdmin();
    }
    
    async Task AuthorizeAdmin()
    {
        ServiceReply<bool> reply = await AdminService.AuthorizeAdmin();
        PageIsAuthorized = reply.Data;

        if ( !PageIsAuthorized )
        {
            Logger.LogError( reply.ErrorType + reply.Message );
            SetViewMessage( false, reply.ErrorType + reply.Message );
            StartPageRedirection( "You are not an admin!" );
        }
    }
}