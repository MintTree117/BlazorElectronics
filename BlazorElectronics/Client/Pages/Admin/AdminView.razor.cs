using BlazorElectronics.Client.Pages.UserViews;
using BlazorElectronics.Client.Services.Admin;
using BlazorElectronics.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.Admin;

public class AdminView : UserView
{
    protected const string NOT_ADMIN_AUTHORIZED_MESSAGE = "You are not an admin!";
    protected const string INVALID_QUERY_PARAMS_MESSAGE = "Invalid url paramters!";
    
    [Inject]
    IAdminServiceClient AdminService { get; init; } = default!;

    protected override async Task OnInitializedAsync()
    {
        GetReturnUrl();
        await AuthorizeAdmin();
    }

    protected async Task AuthorizeAdmin()
    {
        ApiReply<bool> response = await AdminService.AuthorizeAdmin();
        IsAuthorized = response.Data;
        if ( !IsAuthorized )
            Message = NOT_ADMIN_AUTHORIZED_MESSAGE;
    }
}