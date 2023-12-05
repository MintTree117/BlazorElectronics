using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.Admin.Crud;

public sealed partial class CrudCategories : CrudPage<CategoryView, CategoryEdit>
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
    protected override string GetEditTitle()
    {
        return $"Edit {ItemEdit.Tier} {ItemTitle}";
    }
    
    void HandleTierChange( ChangeEventArgs e )
    {
        StateHasChanged();
    }
    List<CategoryView> GetEditParents()
    {
        return Enum.TryParse( ( ( int ) ItemEdit.Tier - 1 ).ToString(), out CategoryTier parentTier )
            ? ItemsView.Where( item => item.Tier == parentTier ).ToList()
            : new List<CategoryView>();
    }
    void SortByTier()
    {
        ItemsView = ItemsView.OrderBy( c => c.Tier ).ToList();
    }
}