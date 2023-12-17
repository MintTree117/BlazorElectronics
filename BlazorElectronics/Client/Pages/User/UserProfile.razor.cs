using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Users;

namespace BlazorElectronics.Client.Pages.User;

public partial class UserProfile : UserPage
{
    readonly PasswordRequestDto _requestDto = new();

    async Task ChangePassword()
    {
        ServiceReply<bool> result = await UserService.ChangePassword( _requestDto );
        
        
    }
}