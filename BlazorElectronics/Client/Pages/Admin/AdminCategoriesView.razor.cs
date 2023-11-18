using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Outbound.Categories;

namespace BlazorElectronics.Client.Pages.Admin;

public partial class AdminCategoriesView
{
    CategoriesResponse? Categories;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if ( !IsAuthorized )
        {
            await HandleRedirection();
            return;
        }

        ApiReply<CategoriesResponse?> reply = await CategoryService.GetCategories();

        IsLoaded = true;
        
        if ( !reply.Success || reply.Data is null )
        {
            Message = reply.Message ??= "Failed to get Categories!";
            return;
        }

        Categories = reply.Data;
    }

    void CreateNewCategory( int categoryTier )
    {
        NavManager.NavigateTo( $"admin/category-edit?newCategory=true&categoryTier={categoryTier}" );
    }
    void EditCategory( short categoryId, int categoryTier )
    {
        NavManager.NavigateTo( $"admin/category-edit?categoryId={categoryId}&categoryTier={categoryTier}" );
    }
    void DeleteCategory( short categoryId, int categoryTier ) { }
}