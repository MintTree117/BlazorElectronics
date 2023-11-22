using BlazorElectronics.Client.Services.Users.Admin;
using BlazorElectronics.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.User.Admin;

public abstract class AdminView : UserView
{
    protected const string ERROR_UNAUTHORIZED_ADMIN = "You are not an admin!";

    [Inject] IAdminServiceClient AdminService { get; init; } = default!;

    protected override async Task OnInitializedAsync()
    {
        GetReturnUrl();
        await AuthorizeAdmin();
    }

    async Task AuthorizeAdmin()
    {
        ApiReply<bool> response = await AdminService.AuthorizeAdmin();
        PageIsAuthorized = response.Data;

        if ( !PageIsAuthorized )
        {
            ViewMessageClass = MESSAGE_FAILURE_CLASS;
            ViewMessage = ERROR_UNAUTHORIZED_ADMIN;   
        }
    }
}