using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Inbound.Users;

namespace BlazorElectronics.Client.Pages.UserViews;

public partial class UserProfile : UserView
{
    const string DEFAULT_FAIL_PASSWORD_CHANGE = "Failed to change password; no response message!";
    
    readonly UserChangePasswordRequest _changePasswordRequest = new();

    protected override async Task OnInitializedAsync()
    {
        GetReturnUrl();
        
        await AuthorizeUser();

        PageIsLoaded = true;

        if ( !PageIsAuthorized )
            StartPageRedirection();
    }

    async Task ChangePassword()
    {
        ApiReply<bool> result = await UserService.ChangePassword( _changePasswordRequest );
        RazorViewMessage = result.Message ??= DEFAULT_FAIL_PASSWORD_CHANGE;
    }
}