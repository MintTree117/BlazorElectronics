using BlazorElectronics.Client.Services.Admin;
using BlazorElectronics.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.UserViews.Admin;

public abstract class AdminView : UserView
{
    protected const string ERROR_UNAUTHORIZED_ADMIN = "You are not an admin!";

    [Inject] IAdminServiceClient AdminService { get; init; } = default!;

    protected override async Task OnInitializedAsync()
    {
        GetReturnUrl();
        await AuthorizeAdmin();
    }

    protected async Task AuthorizeAdmin()
    {
        ApiReply<bool> response = await AdminService.AuthorizeAdmin();
        PageIsAuthorized = response.Data;
        
        if ( !PageIsAuthorized )
            RazorViewMessage = ERROR_UNAUTHORIZED_ADMIN;
    }
}