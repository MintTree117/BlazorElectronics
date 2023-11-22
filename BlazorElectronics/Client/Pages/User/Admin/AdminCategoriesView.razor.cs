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
    
    void CreateNewCategory( CategoryType categoryType )
    {
        NavManager.NavigateTo( $"admin/categories/edit?newCategory=true&categoryTier={categoryType}" );
    }
    void EditCategory( int categoryId, CategoryType categoryType )
    {
        NavManager.NavigateTo( $"admin/categories/edit?newCategory=false&categoryId={categoryId}&categoryTier={categoryType}" );
    }
    async Task RemoveCategory( int categoryId, CategoryType categoryType )
    {
        var dto = new RemoveCategoryDto( categoryId, categoryType );
        ApiReply<bool> result = await AdminCategoryService.RemoveCategory( dto );

        if ( !result.Success )
        {
            SetActionMessage( false, $"Failed to delete {categoryType} category {categoryId}. {result.Message}" );
            return;
        }

        SetActionMessage( true, $"Successfully deleted {categoryType} category {categoryId}." );
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
            SetViewMessage( false, reply.Message ??= ERROR_GET_CATEGORIES_VIEW );
            return;
        }
        
        _category = reply.Data;
        ViewMessage = string.Empty;
    }
}