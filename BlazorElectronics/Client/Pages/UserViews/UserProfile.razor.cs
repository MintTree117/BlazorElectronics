using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Inbound.Users;

namespace BlazorElectronics.Client.Pages.UserViews;

public partial class UserProfile : UserView
{
    protected readonly UserChangePasswordRequest Request = new();

    protected override async Task OnInitializedAsync()
    {
        await AuthorizeUser();

        IsLoaded = true;

        if ( !IsAuthorized )
            await HandleRedirection();
    }

    async Task ChangePassword()
    {
        ApiReply<bool> result = await UserService.ChangePassword( Request );
        Message = result.Message ??= "Failed to change password; no response message!";
    }
}