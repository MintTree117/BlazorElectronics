using BlazorElectronics.Client.Services.Users.Admin;
using BlazorElectronics.Client.Services.Users.Admin.SpecLookups;
using BlazorElectronics.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.User.Admin.SpecLookups;

public sealed partial class AdminSpecsViewPage : AdminViewPage<AdminItemViewDto>
{
    [Inject] IAdminSpecsServiceClient SpecsService { get; init; } = default!;

    protected override async Task OnInitializedAsync()
    {
        if ( !PageIsAuthorized )
        {
            Logger.LogError( ERROR_UNAUTHORIZED_ADMIN );
            StartPageRedirection();
            return;
        }

        UrlItemName = "specs";
        ViewService = SpecsService;

        await LoadView();
    }
}