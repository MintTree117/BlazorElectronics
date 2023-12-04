using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Features;

namespace BlazorElectronics.Client.Pages.User.Admin.Crud;

public sealed partial class CrudFeaturedDeals : CrudPage<AdminItemViewDto, FeaturedDeal>
{
    protected override async Task OnInitializedAsync()
    {
        ItemTitle = "Featured Deal";

        await base.OnInitializedAsync();

        if ( !PageIsAuthorized )
            return;

        ApiPath = "api/AdminFeaturedDeal";
        await LoadView();
    }
}