using BlazorElectronics.Client.Services.Users.Admin;
using BlazorElectronics.Client.Services.Users.Admin.Features;
using BlazorElectronics.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.User.Admin.Features;

public partial class AdminFeaturesViewPage : AdminViewPage<AdminItemViewDto>
{
    [Inject] IAdminFeaturesServiceClient FeaturesService { get; init; } = default!;

    protected override async Task OnInitializedAsync()
    {
        if ( !PageIsAuthorized )
        {
            Logger.LogError( ERROR_UNAUTHORIZED_ADMIN );
            StartPageRedirection();
            return;
        }

        UrlItemName = "features";
        ViewService = FeaturesService;

        await LoadView();
    }
}