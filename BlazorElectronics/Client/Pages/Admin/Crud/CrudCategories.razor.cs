using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Enums;

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
        THeadMeta.Add( "Category Parent", SortByParent );
    }
    protected override string GetEditTitle()
    {
        return $"Edit {ItemEdit.Tier} {ItemTitle}";
    }
    
    public override void CreateItem()
    {
        base.CreateItem();
        ItemEdit.Tier = CategoryTier.Primary;
        ItemEdit.PrimaryId = 1;
        ItemEdit.ParentCategoryId = null;
    }
    List<CategoryView> GetPrimary()
    {
        return ItemsView.Where( c => c.Tier == CategoryTier.Primary ).ToList();
    }
    List<CategoryView> GetEditParents()
    {
        return ItemEdit.Tier == CategoryTier.Tertiary 
            ? ItemsView.Where( c => c.ParentCategoryId == ItemEdit.PrimaryId ).ToList() 
            : ItemsView.Where( c => c.Tier == CategoryTier.Primary ).ToList();
    }
    void SortByTier()
    {
        ItemsView = ItemsView.OrderBy( c => c.Tier ).ToList();
    }
    void SortByParent()
    {
        ItemsView = ItemsView.OrderBy( c => c.ParentCategoryId ).ToList();
    }
}