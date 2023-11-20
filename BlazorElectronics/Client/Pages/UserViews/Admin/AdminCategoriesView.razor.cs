using BlazorElectronics.Client.Services.Admin;
using BlazorElectronics.Client.Services.Categories;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin.Categories;
using BlazorElectronics.Shared.Outbound.Categories;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.UserViews.Admin;

public sealed partial class AdminCategoriesView : AdminView
{
    const string ERROR_GET_CATEGORIES_VIEW = "Failed to retireve Categories View with no message!";
    
    [Inject] IAdminCategoryServiceClient AdminCategoryService { get; init; } = default!;

    CategoryViewDto _category = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if ( !PageIsAuthorized )
        {
            Logger.LogError( ERROR_UNAUTHORIZED_ADMIN );
            StartPageRedirection();
            return;
        }

        ApiReply<CategoryViewDto?> reply = await AdminCategoryService.GetCategoriesView();

        PageIsLoaded = true;
        
        if ( !reply.Success || reply.Data is null )
        {
            Logger.LogError( reply.Message ??= ERROR_GET_CATEGORIES_VIEW );
            RazorViewMessage = reply.Message ??= ERROR_GET_CATEGORIES_VIEW;
            return;
        }

        _category = reply.Data;
    }

    void CreateNewCategory( int categoryTier )
    {
        NavManager.NavigateTo( $"admin/categories/edit?newCategory=true&categoryTier={categoryTier}" );
    }
    void EditCategory( int categoryId, int categoryTier )
    {
        NavManager.NavigateTo( $"admin/categories/edit?newCategory=false&categoryId={categoryId}&categoryTier={categoryTier}" );
    }
    async Task<bool> RemoveCategory( int categoryId, int categoryTier )
    {
        var dto = new RemoveCategoryDto( categoryId, categoryTier );
        ApiReply<bool> result = await AdminCategoryService.RemoveCategory( dto );

        if ( !result.Success )
        {
            RazorViewMessage = result.Message ??= "Failed to delete category!";
            return false;
        }
        
        StateHasChanged();
        NavManager.NavigateTo( NavManager.Uri, true );
        return true;
    }
}