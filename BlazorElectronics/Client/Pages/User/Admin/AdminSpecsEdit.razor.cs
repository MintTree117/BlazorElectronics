using System.Collections.Specialized;
using System.Web;
using BlazorElectronics.Client.Services.Users.Admin;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin.Specs;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.User.Admin;

public sealed partial class AdminSpecsEdit : AdminView
{
    [Inject] IAdminSpecsServiceClient AdminSpecService { get; set; } = default!;
    
    SpecLookupEditDto _dto = new();
    bool _newSpec;

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

        if ( !TryParseUrlParameters( out int specId, out SpecLookupType specType ) )
        {
            SetViewMessage( false, ERROR_INVALID_URL_PARAMS );
            Logger.LogError( ERROR_INVALID_URL_PARAMS );
            StartPageRedirection();
            return;
        }

        if ( _newSpec )
        {
            PageIsLoaded = true;
            _dto.SpecType = specType;
            return;
        }

        var request = new SpecLookupGetEditDto( specType, specId );
        ApiReply<SpecLookupEditDto?> reply = await AdminSpecService.GetEdit( request );

        if ( !reply.Success || reply.Data is null )
        {
            Logger.LogError( reply.Message ??= "Failed to get spec lookup!" );
            SetViewMessage( false, reply.Message ??= "Failed to get spec lookup!" );
            StartPageRedirection();
            return;
        }

        PageIsLoaded = true;
        _dto = reply.Data;
        StateHasChanged();
    }
    bool TryParseUrlParameters( out int specId, out SpecLookupType specType )
    {
        specId = -1;
        specType = SpecLookupType.INT;
        
        Uri uri = NavManager.ToAbsoluteUri( NavManager.Uri );
        NameValueCollection queryString = HttpUtility.ParseQueryString( uri.Query );

        string? newSpecString = queryString.Get( "newSpec" );
        string? specIdString = queryString.Get( "specId" );
        string? specTypeString = queryString.Get( "specType" );

        bool parsed = !string.IsNullOrWhiteSpace( specTypeString ) &&
                      Enum.TryParse( specTypeString, out specType ) &&
                      !string.IsNullOrWhiteSpace( newSpecString ) &&
                      bool.TryParse( newSpecString, out _newSpec );

        if ( _newSpec )
            return parsed;
        
        return !string.IsNullOrWhiteSpace( specIdString ) && int.TryParse( specIdString, out specId );
    }

    async Task Submit()
    {
        if ( _newSpec )
            await SubmitNew();
        else
            await SubmitUpdate();
    }
    async Task SubmitNew()
    {
        ApiReply<int> reply = await AdminSpecService.Add( _dto );

        if ( !reply.Success )
        {
            SetActionMessage( false, reply.Message ??= "Failed to insert spec, no response message!" );
            return;
        }

        _dto.SpecId = reply.Data;
        _newSpec = false;
        
        SetActionMessage( true, "Successfully added spec." );
        StateHasChanged();
    }
    async Task SubmitUpdate()
    {
        ApiReply<bool> reply = await AdminSpecService.Update( _dto );

        if ( !reply.Success )
        {
            SetActionMessage( false, reply.Message ??= "Failed to update spec, no response message!" );
            return;
        }
        
        SetActionMessage( true, "Successfully updated spec." );
        StateHasChanged();
    }
}