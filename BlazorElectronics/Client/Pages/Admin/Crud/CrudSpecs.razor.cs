using BlazorElectronics.Client.Services.Admin;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.SpecLookups;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.Admin.Crud;

public sealed partial class CrudSpecs : CrudPage<CrudView, SpecEdit>
{
    [Inject] public IAdminCategoryHelper CategoryHelper { get; set; } = default!;
    
    protected override async Task OnInitializedAsync()
    {
        ItemTitle = "Spec Lookup";

        await base.OnInitializedAsync();

        if ( !PageIsAuthorized )
            return;

        ServiceReply<bool> categoryResponse = await CategoryHelper.Init();

        if ( !categoryResponse.Success )
        {
            SetActionMessage( false, categoryResponse.Message ?? "Failed to get categories!" );
            return;
        }

        ApiPath = "api/AdminSpecLookup";
        await LoadView();
    }

    protected override async Task Submit()
    {
        ItemEdit.PrimaryCategories = CategoryHelper.GetSelectedPrimaryOptions();
        await base.Submit();
    }
    public override void CreateItem()
    {
        base.CreateItem();
        CategoryHelper.ResetPrimarySelection();
        StateHasChanged();
    }
    public override async Task EditItem( int itemId )
    {
        await base.EditItem( itemId );
        CategoryHelper.SetPrimaryOptions( ItemEdit.PrimaryCategories );
        StateHasChanged();
    }
    void HandleGlobalChange( ChangeEventArgs e )
    {
        ItemEdit.IsGlobal = !ItemEdit.IsGlobal;
        CategoryHelper.ResetPrimarySelection();
        StateHasChanged();
    }
}