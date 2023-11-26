using BlazorElectronics.Client.Services.Users.Admin;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin.Vendors;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.User.Admin;

public partial class AdminVendorsView : AdminView
{
    const string ERROR_GET_VENDORS_VIEW = "Failed to retireve Vendors View with no message!";
    
    [Inject] IAdminVendorServiceClient AdminVendorServiceClient { get; init; } = default!;

    VendorsViewDto _dto = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if ( !PageIsAuthorized )
        {
            Logger.LogError( ERROR_UNAUTHORIZED_ADMIN );
            StartPageRedirection();
            return;
        }

        await LoadView();
    }

    void AddNewVendor()
    {
        NavManager.NavigateTo( "admin/vendors/edit?newVendor=true" );
    }
    void EditVendor( int vendorId )
    {
        NavManager.NavigateTo( $"admin/vendors/edit?newVendor=false&vendorId={vendorId}" );
    }
    async Task RemoveVendor( int vendorId )
    {
        ApiReply<bool> reply = await AdminVendorServiceClient.Remove( new IdDto( vendorId ) );

        if ( !reply.Success )
        {
            SetActionMessage( false, $"Failed to delete vendor! {reply.Message}" );
            return;
        }

        SetActionMessage( true, $"Successfully deleted vendor {vendorId}." );
        await LoadView();
        StateHasChanged();
    }
    async Task LoadView()
    {
        PageIsLoaded = false;

        ApiReply<VendorsViewDto?> reply = await AdminVendorServiceClient.GetView();

        PageIsLoaded = true;

        if ( !reply.Success || reply.Data is null )
        {
            Logger.LogError( reply.Message ??= ERROR_GET_VENDORS_VIEW );
            SetViewMessage( false, reply.Message ??= ERROR_GET_VENDORS_VIEW );
            return;
        }

        _dto = reply.Data;
        ViewMessage = string.Empty;
    }

    void SortById()
    {
        _dto.Vendors = _dto.Vendors.OrderBy( v => v.VendorId ).ToList();
    }
    void SortByName()
    {
        _dto.Vendors = _dto.Vendors.OrderBy( v => v.VendorName ).ToList();
    }
}