using System.Collections.Specialized;
using System.Web;
using BlazorElectronics.Client.Services.Users.Admin;
using BlazorElectronics.Client.Services.Users.Admin.Vendors;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Vendors;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.User.Admin.Vendors;

public partial class AdminVendorsEdit : AdminPage
{
    [Inject] IAdminVendorServiceClient AdminVendorServiceClient { get; set; } = default!;

    bool _newVendor;
    VendorEditDto _dto = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if ( !PageIsAuthorized )
        {
            Logger.LogError( ERROR_UNAUTHORIZED_ADMIN );
            StartPageRedirection();
            return;
        }

        if ( !TryParseUrlParameters( out int variantId ) )
        {
            SetViewMessage( false, ERROR_INVALID_URL_PARAMS );
            Logger.LogError( ERROR_INVALID_URL_PARAMS );
            StartPageRedirection();
            return;
        }

        if ( _newVendor )
        {
            PageIsLoaded = true;
            return;
        }

        ServiceReply<VendorEditDto?> reply = await AdminVendorServiceClient.GetEdit( new IntDto( variantId ) );

        if ( !reply.Success || reply.Data is null )
        {
            Logger.LogError( reply.Message ?? "Failed to get vendor!" );
            SetViewMessage( false, reply.Message ?? "Failed to get vendor!" );
            StartPageRedirection();
            return;
        }

        PageIsLoaded = true;
        _dto = reply.Data;
        StateHasChanged();
    }
    bool TryParseUrlParameters( out int vendorId )
    {
        vendorId = -1;

        Uri uri = NavManager.ToAbsoluteUri( NavManager.Uri );
        NameValueCollection queryString = HttpUtility.ParseQueryString( uri.Query );

        string? newVendorString = queryString.Get( "newVendor" );
        string? vendorIdString = queryString.Get( "vendorId" );

        bool parsed = !string.IsNullOrWhiteSpace( newVendorString ) &&
                      bool.TryParse( newVendorString, out _newVendor );

        if ( _newVendor )
            return parsed;

        return !string.IsNullOrWhiteSpace( vendorIdString ) && int.TryParse( vendorIdString, out vendorId );
    }

    async Task Submit()
    {
        if ( _newVendor )
            await SubmitNew();
        else
            await SubmitEdit();
    }
    async Task SubmitNew()
    {
        ServiceReply<int> reply = await AdminVendorServiceClient.Add( _dto );

        if ( !reply.Success )
        {
            SetActionMessage( false, $"Failed to add vendor! {reply.Message}" );
            return;
        }

        _newVendor = false;
        _dto.VendorId = reply.Data;

        SetActionMessage( true, $"Successfully added vendor." );
        StateHasChanged();
    }
    async Task SubmitEdit()
    {
        ServiceReply<bool> reply = await AdminVendorServiceClient.Update( _dto );

        if ( !reply.Success )
        {
            SetActionMessage( false, $"Failed to update vendor! {reply.Message}" );
            return;
        }

        SetActionMessage( true, $"Successfully updated vendor." );
        StateHasChanged();
    }
}