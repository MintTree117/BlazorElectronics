using BlazorElectronics.Client.Services.Users.Admin;
using BlazorElectronics.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.User.Admin.Features;

public partial class AdminFeaturesView : AdminView<AdminItemViewDto>
{
    [Inject] IAdminFeaturedProductsServiceClient FeaturesService { get; init; } = default!;

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