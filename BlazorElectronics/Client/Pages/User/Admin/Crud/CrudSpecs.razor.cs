using BlazorElectronics.Shared;
using BlazorElectronics.Shared.SpecLookups;

namespace BlazorElectronics.Client.Pages.User.Admin.Crud;

public sealed partial class CrudSpecs : CrudPage<AdminItemViewDto, SpecLookupEditDto>
{
    protected override async Task OnInitializedAsync()
    {
        ItemTitle = "Spec";

        await base.OnInitializedAsync();

        if ( !PageIsAuthorized )
            return;

        ApiPath = "api/AdminSpecs";
        await LoadView();
    }
}