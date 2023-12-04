using BlazorElectronics.Shared.Categories;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.User.Admin.Crud;

public sealed partial class CrudCategories : CrudPage<CategoryViewDto, CategoryEditDto>
{
    protected override async Task OnInitializedAsync()
    {
        ItemTitle = "Category";
        
        await base.OnInitializedAsync();

        if ( !PageIsAuthorized )
            return;
        
        ApiPath = "api/AdminCategory";
        await LoadView();
    }

    protected override void GenerateTableMeta()
    {
        base.GenerateTableMeta();
        THeadMeta.Add( "Category Tier", SortByTier );
    }

    void HandleTierChange( ChangeEventArgs e )
    {
        StateHasChanged();
    }
    List<CategoryViewDto> GetEditParents()
    {
        return ItemsView.Where( item => item.Tier == ItemEdit.Tier ).ToList();
    }
    void SortByTier()
    {
        ItemsView = ItemsView.OrderBy( c => c.Tier ).ToList();
    }
}