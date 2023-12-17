using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Features;

namespace BlazorElectronics.Client.Pages.Admin.Crud;

public sealed partial class CrudFeaturedDeals : CrudPage<CrudViewDto, FeaturedDealDtoEditDto>
{
    protected override async Task OnInitializedAsync()
    {
        ItemTitle = "Featured Deal";

        await base.OnInitializedAsync();

        if ( !PageIsAuthorized )
            return;

        ApiPath = "api/AdminFeaturedDeals";
        await LoadView();
    }
}