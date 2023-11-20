using BlazorElectronics.Client.Services.Admin;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin.Specs;
using BlazorElectronics.Shared.Admin.Specs.SpecsSingle;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.UserViews.Admin;

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

        ApiReply<SpecsViewDto?> reply = await AdminSpecsService.GetSpecsView();

        PageIsLoaded = true;

        if ( !reply.Success || reply.Data is null )
        {
            Logger.LogError( reply.Message ??= ERROR_GET_SPECS_VIEW );
            RazorViewMessage = reply.Message ??= ERROR_GET_SPECS_VIEW;
            return;
        }
        
        _specs = reply.Data;
    }

    void CreateNewSpec( SpecLookupType specType )
    {
        
    }
    void EditSpec( SpecLookupType specType, int specId )
    {
        
    }
    async Task RemoveSpec( SpecLookupType specType, int specId )
    {
        
    }
}