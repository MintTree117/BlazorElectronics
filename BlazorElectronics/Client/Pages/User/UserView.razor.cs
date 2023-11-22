using BlazorElectronics.Client.Services.Users;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Outbound.Users;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.User;

public abstract class UserView : PageView
{
    protected const string ERROR_UNAUTHORIZED_USER = "You are not authorized to be here!";
    
    [Inject] protected IUserServiceClient UserService { get; init; } = default!;
    protected bool PageIsAuthorized = false;

    protected sealed override bool PageIsReady()
    {
        return PageIsAuthorized && PageIsLoaded;
    }
    protected async Task AuthorizeUser()
    {
        ApiReply<UserSessionResponse?> response = await UserService.AuthorizeUser();
        PageIsAuthorized = response.Data is not null;
        
        if ( !PageIsAuthorized )
            Message = ERROR_UNAUTHORIZED_USER;
    }
}