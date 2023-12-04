using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Features;

namespace BlazorElectronics.Client.Pages.Admin.Crud;

public sealed partial class CrudFeatures : CrudPage<CrudView, FeatureEdit>
{
    protected override async Task OnInitializedAsync()
    {
        ItemTitle = "Feature";
        
        await base.OnInitializedAsync();

        if ( !PageIsAuthorized )
            return;

        ApiPath = "api/AdminFeatures";
        await LoadView();
    }
}