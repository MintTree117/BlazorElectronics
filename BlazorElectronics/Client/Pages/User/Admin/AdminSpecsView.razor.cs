using BlazorElectronics.Client.Services.Users.Admin;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin.SpecLookups;
using BlazorElectronics.Shared.SpecLookups;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.User.Admin;

public sealed partial class AdminSpecsView : AdminView
{
    const string ERROR_GET_SPECS_VIEW = "Failed to retireve Specs View with no message!";
    
    [Inject] IAdminSpecsServiceClient AdminSpecsService { get; init; } = default!;
    
    List<SpecLookupViewDto> _specs = new();
    
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

    void CreateNewSpec()
    {
        NavManager.NavigateTo( $"admin/specs/edit?newSpec=true" );
    }
    void EditSpec( int specId )
    {
        NavManager.NavigateTo( $"admin/specs/edit?newSpec=false&specId={specId}" );
    }
    async Task RemoveSpec( int specId )
    {
        ApiReply<bool> reply = await AdminSpecsService.Remove( new IdDto( specId ) );

        if ( !reply.Success )
        {
            SetActionMessage( false, $"Failed to remove Spec Lookup {specId}! {reply.Message}" );
            StateHasChanged();
            return;
        }
        
        SetActionMessage( true, $"Successfully removed Spec Lookup {specId}." );
        await LoadSpecsView();
    }
    async Task LoadSpecsView()
    {
        PageIsLoaded = false;
        
        ApiReply<List<SpecLookupViewDto>?> reply = await AdminSpecsService.GetView();

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

    void SortById()
    {
        _specs = _specs.OrderBy( s => s.SpecId ).ToList();
    }
    void SortByName()
    {
        _specs = _specs.OrderBy( s => s.SpecName ).ToList();
    }
}