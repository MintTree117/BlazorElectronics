using BlazorElectronics.Client.Services.Users.Admin;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin.Categories;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.User.Admin;

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

        await LoadCategoriesView();
    }

    void CreateNewCategory( int categoryTier )
    {
        NavManager.NavigateTo( $"admin/categories/edit?newCategory=true&categoryTier={categoryTier}" );
    }
    void EditCategory( int categoryId, int categoryTier )
    {
        NavManager.NavigateTo( $"admin/categories/edit?newCategory=false&categoryId={categoryId}&categoryTier={categoryTier}" );
    }
    async Task RemoveCategory( int categoryId, int categoryTier )
    {
        var dto = new RemoveCategoryDto( categoryId, categoryTier );
        ApiReply<bool> result = await AdminCategoryService.RemoveCategory( dto );

        if ( !result.Success )
        {
            MessageCssClass = MESSAGE_FAILURE_CLASS;
            Message = $"Failed to delete category {categoryId}. {result.Message}";
            return;
        }

        MessageCssClass = MESSAGE_SUCCESS_CLASS;
        Message = $"Successfully deleted category {categoryId}.";

        await LoadCategoriesView();
        StateHasChanged();
    }
    async Task LoadCategoriesView()
    {
        PageIsLoaded = false;
        
        ApiReply<CategoryViewDto?> reply = await AdminCategoryService.GetCategoriesView();

        PageIsLoaded = true;

        if ( !reply.Success || reply.Data is null )
        {
            Logger.LogError( reply.Message ??= ERROR_GET_CATEGORIES_VIEW );
            MessageCssClass = MESSAGE_FAILURE_CLASS;
            Message = reply.Message ??= ERROR_GET_CATEGORIES_VIEW;
            return;
        }
        
        _category = reply.Data;
        Message = string.Empty;
    }
}