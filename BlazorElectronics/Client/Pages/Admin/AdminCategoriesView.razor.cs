using BlazorElectronics.Client.Services.Admin;
using BlazorElectronics.Client.Services.Categories;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin.Categories;
using BlazorElectronics.Shared.Outbound.Categories;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.Admin;

public partial class AdminCategoriesView
{
    [Inject] IAdminCategoryServiceClient AdminCategoryService { get; init; } = default!;
    [Inject] ICategoryServiceClient CategoryService { get; init; } = default!;
    
    CategoriesResponse? Categories;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if ( !IsAuthorized )
        {
            Logger.LogError( "Not authorized!" );
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
        NavManager.NavigateTo( $"admin/categories/edit?newCategory=true&categoryTier={categoryTier}" );
    }
    void EditCategory( short categoryId, int categoryTier )
    {
        NavManager.NavigateTo( $"admin/categories/edit?newCategory=false&categoryId={categoryId}&categoryTier={categoryTier}" );
    }
    async Task<bool> RemoveCategory( short categoryId, int categoryTier )
    {
        var dto = new DeleteCategoryDto( categoryId, categoryTier );
        ApiReply<bool> result = await AdminCategoryService.RemoveCategory( dto );

        if ( !result.Success )
        {
            Message = result.Message ??= "Failed to delete category!";
            return false;
        }
        
        StateHasChanged();
        NavManager.NavigateTo( NavManager.Uri, true );
        return true;
    }
}