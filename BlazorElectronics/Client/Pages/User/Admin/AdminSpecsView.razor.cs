using BlazorElectronics.Client.Services.Users.Admin;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin.Specs;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.User.Admin;

public sealed partial class AdminSpecsView : AdminView
{
    const string ERROR_GET_SPECS_VIEW = "Failed to retireve Specs View with no message!";
    
    [Inject] IAdminSpecsServiceClient AdminSpecsService { get; init; } = default!;
    
    SpecsViewDto _specs = new();
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if ( !PageIsAuthorized )
        {
            Logger.LogError( ERROR_UNAUTHORIZED_ADMIN );
            StartPageRedirection();
            return;
        }

        await LoadSpecsView();
    }

    void CreateNewSpec( SpecLookupType specType )
    {
        NavManager.NavigateTo( $"admin/specs/edit?newSpec=true&specType={specType}" );
    }
    void EditSpec( SpecLookupType specType, int specId )
    {
        NavManager.NavigateTo( $"admin/specs/edit?newSpec=false&specType={specType}&specId={specId}" );
    }
    async Task RemoveSpec( SpecLookupType specType, int specId )
    {
        var dto = new RemoveSpecLookupDto( specType, specId );
        ApiReply<bool> reply = await AdminSpecsService.Remove( dto );

        if ( !reply.Success )
        {
            SetActionMessage( false, $"Failed to remove {specType} spec {specId}. {reply.Message}" );
            StateHasChanged();
            return;
        }
        
        SetActionMessage( true, $"Successfully removed {specType} spec {specId}." );
        await LoadSpecsView();
    }
    async Task LoadSpecsView()
    {
        PageIsLoaded = false;
        
        ApiReply<SpecsViewDto?> reply = await AdminSpecsService.GetView();

        PageIsLoaded = true;

        if ( !reply.Success || reply.Data is null )
        {
            Logger.LogError( reply.Message ??= ERROR_GET_SPECS_VIEW );
            SetViewMessage( false, reply.Message ??= ERROR_GET_SPECS_VIEW );
            return;
        }

        _specs = reply.Data;
        ViewMessage = string.Empty;
    }
}