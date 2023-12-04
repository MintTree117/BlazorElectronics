using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Vendors;

namespace BlazorElectronics.Client.Pages.User.Admin.Crud;

public sealed partial class CrudVendors : CrudPage<AdminItemViewDto, VendorEditDto>
{
    protected override async Task OnInitializedAsync()
    {
        ItemTitle = "Vendor";

        await base.OnInitializedAsync();

        if ( !PageIsAuthorized )
            return;

        ApiPath = "api/AdminVendors";
        await LoadView();
    }
}