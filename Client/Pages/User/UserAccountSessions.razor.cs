using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Sessions;

namespace BlazorElectronics.Client.Pages.User;

public sealed partial class UserAccountSessions : UserPage
{
    List<SessionInfoDto> _sessions = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await LoadSessions();
        PageIsLoaded = true;
    }

    async Task LoadSessions()
    {
        ServiceReply<List<SessionInfoDto>?> reply = await UserService.GetUserSessions();

        if ( !reply.Success || reply.Data is null )
        {
            InvokeAlert( AlertType.Danger, $"Failed to get sessions! {reply.ErrorType} : {reply.Message}" );
            return;
        }

        _sessions = reply.Data;
        StateHasChanged();
    }
    async Task DeleteSession( int sessionId )
    {
        SessionInfoDto? session = _sessions.Find( s => s.SessionId == sessionId );

        if ( session is null )
            return;
        
        ServiceReply<bool> reply = await UserService.DeleteSession( sessionId );

        if ( !reply.Success )
        {
            InvokeAlert( AlertType.Danger, $"Failed to delete session! {reply.ErrorType} : {reply.Message}" );
            return;
        }

        _sessions.Remove( session );
        InvokeAlert( AlertType.Success, "Successfully deleted session." );
        StateHasChanged();
    }
    async Task ClearSessions()
    {
        ServiceReply<bool> reply = new ServiceReply<bool>( false ); //await UserService.DeleteAllSessions();

        if ( !reply.Success )
        {
            InvokeAlert( AlertType.Danger, $"Failed to delete sessions! {reply.ErrorType} : {reply.Message}" );
            return;
        }

        await UserService.Logout();
    }
}