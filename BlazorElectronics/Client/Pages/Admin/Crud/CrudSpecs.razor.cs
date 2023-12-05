using BlazorElectronics.Client.Models;
using BlazorElectronics.Client.Services.Admin;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.SpecLookups;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.Admin.Crud;

public sealed partial class CrudSpecs : CrudPage<CrudView, SpecLookupEdit>
{
    [Inject] IAdminCrudService<CategoryView, CategoryEdit> CategoryService { get; set; } = null!;
    CategorySelection CategoryOptions = null!;

    protected override async Task OnInitializedAsync()
    {
        ItemTitle = "Spec Lookup";

        await base.OnInitializedAsync();

        if ( !PageIsAuthorized )
            return;

        ServiceReply<List<CategoryView>?> categoryResponse = await CategoryService.GetView( "api/AdminCategory/get-view" );

        if ( !categoryResponse.Success || categoryResponse.Data is null )
        {
            Logger.LogError( categoryResponse.ErrorType + categoryResponse.Message );
            return;
        }

        List<CategoryView> categories = categoryResponse.Data.Where( c => c.Tier == CategoryTier.Primary ).ToList();
        Logger.LogError( categories.Count.ToString() );
        CategoryOptions = new CategorySelection( categories );

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