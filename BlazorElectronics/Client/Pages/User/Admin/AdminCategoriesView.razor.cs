using BlazorElectronics.Client.Services.Users.Admin;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.User.Admin;

public sealed partial class AdminCategoriesView : AdminView
{
    const string ERROR_GET_CATEGORIES_VIEW = "Failed to retireve Categories View with no message!";
    
    [Inject] IAdminCategoryServiceClient AdminCategoryService { get; init; } = default!;

    CategoriesViewDto _categories = new();
    CategoryType _activeCategory = CategoryType.PRIMARY;

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

    void SetActiveCategory( CategoryType type )
    {
        _activeCategory = type;
    }
    void SortById( CategoryType list, CategoryType categoryId )
    {
        List<CategoryViewDto> categories = list switch
        {
            CategoryType.PRIMARY => _categories.Primary,
            CategoryType.SECONDARY => _categories.Secondary,
            CategoryType.TERTIARY => _categories.Tertiary,
            _ => throw new ArgumentOutOfRangeException( nameof( list ), list, null )
        };
        
        categories = categoryId switch
        {
            CategoryType.PRIMARY => categories.OrderBy( c => c.PrimaryCategoryId ).ToList(),
            CategoryType.SECONDARY => categories.OrderBy( c => c.SecondaryCategoryId ).ToList(),
            CategoryType.TERTIARY => categories.OrderBy( c => c.TertiaryCategoryId ).ToList(),
            _ => throw new ArgumentOutOfRangeException( nameof( categoryId ), categoryId, null )
        };
        
        switch ( list )
        {
            case CategoryType.PRIMARY:
                _categories.Primary = categories;
                break;
            case CategoryType.SECONDARY:
                _categories.Secondary = categories;
                break;
            case CategoryType.TERTIARY:
                _categories.Tertiary = categories;
                break;
            default:
                throw new ArgumentOutOfRangeException( nameof( list ), list, null );
        }

        StateHasChanged();
    }
    void SortByName( CategoryType list )
    {
        switch ( list )
        {
            case CategoryType.PRIMARY:
                _categories.Primary = _categories.Primary.OrderBy( c => c.Name ).ToList();
                break;
            case CategoryType.SECONDARY:
                _categories.Secondary = _categories.Secondary.OrderBy( c => c.Name ).ToList();
                break;
            case CategoryType.TERTIARY:
                _categories.Tertiary = _categories.Tertiary.OrderBy( c => c.Name ).ToList();
                break;
            default:
                throw new ArgumentOutOfRangeException( nameof( list ), list, null );
        }
        
        StateHasChanged();
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
        var dto = new CategoryRemoveDto( categoryId, categoryType );
        ServiceReply<bool> result = await AdminCategoryService.RemoveCategory( dto );

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
        
        ServiceReply<CategoriesViewDto?> reply = await AdminCategoryService.GetCategoriesView();

        PageIsLoaded = true;

        if ( !reply.Success || reply.Data is null )
        {
            Logger.LogError( reply.Message ?? ERROR_GET_CATEGORIES_VIEW );
            SetViewMessage( false, reply.Message ?? ERROR_GET_CATEGORIES_VIEW );
            return;
        }
        
        _categories = reply.Data;
        ViewMessage = string.Empty;
    }
}