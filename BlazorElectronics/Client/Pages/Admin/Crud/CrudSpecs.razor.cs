using BlazorElectronics.Client.Models;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.SpecLookups;

namespace BlazorElectronics.Client.Pages.Admin.Crud;

public sealed partial class CrudSpecs : CrudPage<CrudView, SpecLookupEdit>
{
    CategorySelection CategoryOptions = new();

    protected override async Task OnInitializedAsync()
    {
        ItemTitle = "Spec Lookup";

        await base.OnInitializedAsync();

        if ( !PageIsAuthorized )
            return;

        ApiPath = "api/AdminSpecLookup";
        await LoadView();
    }

    protected override async Task Submit()
    {
        ItemEdit.PrimaryCategories = CategoryOptions.GetSelected();
        await base.Submit();
    }
    public override async Task EditItem( int itemId )
    {
        await base.EditItem( itemId );
        CategoryOptions.Set( ItemEdit.PrimaryCategories );
        StateHasChanged();
    }
}