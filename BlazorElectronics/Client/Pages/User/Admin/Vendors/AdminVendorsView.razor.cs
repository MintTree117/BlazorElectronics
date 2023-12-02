using BlazorElectronics.Client.Services.Users.Admin;
using BlazorElectronics.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.User.Admin.Vendors;

public partial class AdminVendorsView : AdminView<AdminItemViewDto>
{
    [Inject] IAdminVendorServiceClient VendorService { get; init; } = default!;

    protected override async Task OnInitializedAsync()
    {
        if ( !PageIsAuthorized )
        {
            Logger.LogError( ERROR_UNAUTHORIZED_ADMIN );
            StartPageRedirection();
            return;
        }

        UrlItemName = "categories";
        ViewService = VendorService;

        await LoadView();
    }
}