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
    
    EditSpecLookupDto _dto = new();
    
    bool _newSpec;
    int _specId;
    SpecLookupType _specType;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if ( !PageIsAuthorized )
        {
            Logger.LogError( "Not authorized!" );
            StartPageRedirection();
            return;
        }

        if ( !TryParseUrlParameters() )
        {
            Message = ERROR_INVALID_URL_PARAMS;
            Logger.LogError( ERROR_INVALID_URL_PARAMS );
            StartPageRedirection();
            return;
        }

        if ( _newSpec )
        {
            PageIsLoaded = true;
            _dto.SpecType = _specType;
            return;
        }

        var request = new GetSpecLookupEditDto( _specType, _specId );
        ApiReply<EditSpecLookupDto?> reply = await AdminSpecService.GetEdit( request );

        if ( !reply.Success || reply.Data is null )
        {
            Logger.LogError( reply.Message ??= "Failed to get spec lookup!" );
            Message = reply.Message ??= "Failed to get spec lookup!";
            StartPageRedirection();
            return;
        }

        PageIsLoaded = true;
        _dto = reply.Data;
        StateHasChanged();
    }
    
    bool TryParseUrlParameters()
    {
        Uri uri = NavManager.ToAbsoluteUri( NavManager.Uri );
        NameValueCollection queryString = HttpUtility.ParseQueryString( uri.Query );

        string? newSpecString = queryString.Get( "newSpec" );
        string? specIdString = queryString.Get( "specId" );
        string? specTypeString = queryString.Get( "specType" );

        bool parsed = !string.IsNullOrWhiteSpace( specTypeString ) &&
                      Enum.TryParse( specTypeString, out _specType ) &&
                      !string.IsNullOrWhiteSpace( newSpecString ) &&
                      bool.TryParse( newSpecString, out _newSpec );

        if ( !parsed )
            return false;

        if ( !_newSpec )
            parsed = !string.IsNullOrWhiteSpace( specIdString ) && int.TryParse( specIdString, out _specId );

        return parsed;
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
            MessageCssClass = MESSAGE_FAILURE_CLASS;
            Message = reply.Message ??= "Failed to insert spec, no response message!";
            return;
        }

        _dto.SpecId = reply.Data;
        _specId = _dto.SpecId;
        _newSpec = false;
        
        MessageCssClass = MESSAGE_SUCCESS_CLASS;
        Message = "Successfully added spec.";
        
        StateHasChanged();
    }
    async Task SubmitUpdate()
    {
        ApiReply<bool> reply = await AdminSpecService.Update( _dto );

        if ( !reply.Success )
        {
            MessageCssClass = MESSAGE_FAILURE_CLASS;
            Message = reply.Message ??= "Failed to update spec, no response message!";
            return;
        }

        MessageCssClass = MESSAGE_SUCCESS_CLASS;
        Message = "Successfully updated spec.";
        
        StateHasChanged();
    }
}