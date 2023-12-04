using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Vendors;

namespace BlazorElectronics.Client.Pages.Admin.Crud;

public sealed partial class CrudVendors : CrudPage<CrudView, VendorEdit>
{
    protected override async Task OnInitializedAsync()
    {
        ItemTitle = "Vendor";

        await base.OnInitializedAsync();

        if ( !PageIsAuthorized )
            return;

        ApiPath = "api/AdminVendor";
        await LoadView();
    }
}