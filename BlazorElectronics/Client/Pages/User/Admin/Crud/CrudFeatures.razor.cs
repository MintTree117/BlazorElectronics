using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Features;

namespace BlazorElectronics.Client.Pages.User.Admin.Crud;

public sealed partial class CrudFeatures : CrudPage<AdminItemViewDto, Feature>
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