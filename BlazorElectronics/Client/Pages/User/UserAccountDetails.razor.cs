using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Users;

namespace BlazorElectronics.Client.Pages.User;

public partial class UserAccountDetails : UserPage
{
    AccountDetailsDto _detailsDto = new();
    readonly PasswordRequestDto _passwordDto = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        ServiceReply<AccountDetailsDto?> reply = await UserService.GetAccountDetails();

        if ( !reply.Success || reply.Data is null )
        {
            SetViewMessage( false, $"Failed to retrieve account details! {reply.ErrorType} : {reply.Message}" );
            return;
        }

        _detailsDto = reply.Data;
        PageIsLoaded = true;
        StateHasChanged();
    }

    async Task UpdateAccount()
    {
        ServiceReply<AccountDetailsDto?> reply = await UserService.UpdateAccountDetails( _detailsDto );

        if ( !reply.Success || reply.Data is null )
        {
            InvokeAlert( AlertType.Danger, $"Failed to update account! {reply.ErrorType} : {reply.Message}" );
            return;
        }

        _detailsDto = reply.Data;
        InvokeAlert( AlertType.Success, "Successfully updated you account." );
        StateHasChanged();
    }
    async Task UpdatePassword()
    {
        ServiceReply<bool> reply = await UserService.ChangePassword( _passwordDto );

        if ( !reply.Success )
        {
            InvokeAlert( AlertType.Danger, $"Failed to update password! {reply.ErrorType} : {reply.Message}" );
            return;
        }
        
        InvokeAlert( AlertType.Success, "Successfully updated you password." );
        StateHasChanged();
    }
}