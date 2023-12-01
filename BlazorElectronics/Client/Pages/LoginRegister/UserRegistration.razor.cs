using BlazorElectronics.Client.Services.Users;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Users;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.LoginRegister;

public partial class UserRegistration : PageView
{
    [Inject] IUserServiceClient UserService { get; set; } = default!;
    readonly UserRegisterRequest _user = new();
    bool _registered = false;

    async Task HandleRegistration()
    {
        ServiceReply<UserSessionResponse?> reply = await UserService.Register( _user );

        if ( !reply.Success || reply.Data is null )
        {
            SetActionMessage( false, reply.Message ?? "Failed to register!" );
            return;
        }
        
        _registered = true;
        SetActionMessage( true, "Successfully registered account!" );
        StateHasChanged();
    }
}