using System.Collections.Specialized;
using System.Web;
using BlazorElectronics.Client.Services.Users.Admin;
using BlazorElectronics.Shared;

namespace BlazorElectronics.Client.Pages.User.Admin;

public class AdminEditPage<T> : AdminPage
{
    const string ERROR_GET = "Failed to get item for edit!";
    
    IAdminEditService<T> EditService;

    T _dto;
    bool _newItem;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if ( !PageIsAuthorized )
        {
            Logger.LogError( ERROR_UNAUTHORIZED_ADMIN );
            SetViewMessage( false, ERROR_UNAUTHORIZED_ADMIN );
            StartPageRedirection();
            return;
        }

        if ( !TryParseUrlParameters( out int specId ) )
        {
            SetViewMessage( false, ERROR_INVALID_URL_PARAMS );
            Logger.LogError( ERROR_INVALID_URL_PARAMS );
            StartPageRedirection();
            return;
        }

        if ( _newItem )
        {
            PageIsLoaded = true;
            return;
        }

        ServiceReply<T?> reply = await EditService.GetEdit( new IntDto( specId ) );

        if ( !reply.Success || reply.Data is null )
        {
            Logger.LogError( reply.Message ?? ERROR_GET );
            SetViewMessage( false, reply.Message ?? ERROR_GET );
            StartPageRedirection();
            return;
        }

        PageIsLoaded = true;
        _dto = reply.Data;
        StateHasChanged();
    }

    bool TryParseUrlParameters( out int itemId )
    {
        itemId = -1;

        Uri uri = NavManager.ToAbsoluteUri( NavManager.Uri );
        NameValueCollection queryString = HttpUtility.ParseQueryString( uri.Query );

        string? newSpecString = queryString.Get( "newItem" );
        string? specIdString = queryString.Get( "itemId" );

        bool parsed = !string.IsNullOrWhiteSpace( newSpecString ) &&
                      bool.TryParse( newSpecString, out _newItem );

        if ( _newItem )
            return parsed;

        return !string.IsNullOrWhiteSpace( specIdString ) && int.TryParse( specIdString, out itemId );
    }

    async Task Submit()
    {
        if ( _newItem )
            await SubmitNew();
        else
            await SubmitUpdate();
    }
    async Task SubmitNew()
    {
        ServiceReply<T?> reply = await EditService.Add( _dto );

        if ( !reply.Success || reply.Data is null )
        {
            SetActionMessage( false, reply.Message ?? "Failed to insert spec, no response message!" );
            return;
        }

        _dto = reply.Data;
        _newItem = false;

        SetActionMessage( true, "Successfully added spec." );
        StateHasChanged();
    }
    async Task SubmitUpdate()
    {
        ServiceReply<bool> reply = await EditService.Update( _dto );

        if ( !reply.Success )
        {
            SetActionMessage( false, reply.Message ?? "Failed to update spec, no response message!" );
            return;
        }

        SetActionMessage( true, "Successfully updated spec." );
        StateHasChanged();
    }
}