using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Users;

namespace BlazorElectronics.Client.Pages.User;

public partial class UserProfile : UserPage
{
    readonly PasswordChangeRequest _changeRequest = new();

    async Task ChangePassword()
    {
        ServiceReply<bool> result = await UserService.ChangePassword( _changeRequest );
        
        
    }
}