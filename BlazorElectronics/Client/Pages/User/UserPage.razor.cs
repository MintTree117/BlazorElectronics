using BlazorElectronics.Client.Services.Users;
using BlazorElectronics.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.User;

public abstract class UserPage : PageView
{
    [Inject] protected IUserServiceClient UserService { get; init; } = default!;
    protected bool PageIsAuthorized = false;

    protected override async Task OnInitializedAsync()
    {
        GetReturnUrl();
        await AuthorizeUser();
    }

    public sealed override bool PageIsReady()
    {
        return PageIsAuthorized && PageIsLoaded;
    }
    async Task AuthorizeUser()
    {
        ServiceReply<bool> reply = await UserService.AuthorizeUser();
        PageIsAuthorized = reply.Success;

        if ( !PageIsAuthorized )
        {
            Logger.LogError( reply.ErrorType + reply.Message );
            StartPageRedirection( $"You are not logged in! {reply.ErrorType} : {reply.Message}" );
        }
    }
}