using BlazorElectronics.Client.Pages.UserViews;
using BlazorElectronics.Client.Services.Admin;
using BlazorElectronics.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.Admin;

public class AdminView : UserView
{
    const string NOT_ADMIN_AUTHORIZED_MESSAGE = "You are not an admin!";
    
    [Inject]
    IAdminServiceClient AdminService { get; init; } = default!;

    protected override async Task OnInitializedAsync()
    {
        GetReturnUrl();
        await AuthorizeAdmin();
    }

    protected async Task AuthorizeAdmin()
    {
        ApiReply<bool> response = await AdminService.AuthorizeAdminView();
        IsAuthorized = response.Data;
        if ( !IsAuthorized )
            Message = NOT_ADMIN_AUTHORIZED_MESSAGE;
    }
}