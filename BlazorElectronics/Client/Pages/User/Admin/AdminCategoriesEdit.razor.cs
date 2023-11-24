using System.Collections.Specialized;
using System.Web;
using BlazorElectronics.Client.Services.Users.Admin;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin.Categories;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.User.Admin;

public sealed partial class AdminCategoriesEdit : AdminView
{
    [Inject] IAdminCategoryServiceClient AdminCategoryService { get; init; } = default!;
    
    bool _newCategory;
    int _categoryId = -1;
    CategoryType _categoryType = CategoryType.PRIMARY;

    CategoryEditDto _dto = new();
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if ( !PageIsAuthorized )
        {
            Logger.LogError( ERROR_UNAUTHORIZED_ADMIN );
            StartPageRedirection();
            return;
        }

        if ( !TryParseUrlParameters() )
        {
            SetViewMessage( false, ERROR_INVALID_URL_PARAMS );
            Logger.LogError( ERROR_INVALID_URL_PARAMS );
            StartPageRedirection();
            return;
        }

        if ( _newCategory )
        {
            PageIsLoaded = true;
            _dto.Type = _categoryType;
            return;
        }

        var request = new CategoryGetEditDto( _categoryType, _categoryId );
        ApiReply<CategoryEditDto?> reply = await AdminCategoryService.GetCategoryEdit( request );

        if ( !reply.Success || reply.Data is null )
        {
            Logger.LogError( reply.Message ??= "Failed to get category!" );
            SetViewMessage( false, reply.Message ??= "Failed to get category!" );
            StartPageRedirection();
            return;
        }
        
        PageIsLoaded = true;
        _dto = reply.Data;
        Logger.LogError( _dto.Type.ToString() );
        StateHasChanged();
    }
    
    bool TryParseUrlParameters()
    {
        Uri uri = NavManager.ToAbsoluteUri( NavManager.Uri );
        NameValueCollection queryString = HttpUtility.ParseQueryString( uri.Query );

        string? newCategoryString = queryString.Get( "newCategory" );
        string? categoryIdString = queryString.Get( "categoryId" );
        string? categoryTierString = queryString.Get( "categoryTier" );

        bool parsed = !string.IsNullOrWhiteSpace( categoryTierString ) &&
                      Enum.TryParse( categoryTierString, out _categoryType ) &&
                      !string.IsNullOrWhiteSpace( newCategoryString ) &&
                      bool.TryParse( newCategoryString, out _newCategory );

        if ( !parsed )
            return false;

        if ( !_newCategory )
            parsed = !string.IsNullOrWhiteSpace( categoryIdString ) && int.TryParse( categoryIdString, out _categoryId );

        return parsed;
    }

    async Task Submit()
    {
        if ( _newCategory )
            await SubmitNew();
        else
            await SubmitEdit();
    }
    async Task SubmitNew()
    {
        var request = new CategoryAddDto( _dto );
        ApiReply<CategoryEditDto?> reply = await AdminCategoryService.AddCategory( request );
        
        if ( !reply.Success || reply.Data is null )
        {
            SetActionMessage( false, $"Failed to add {_dto.Type} category! {reply.Message}" );
            return;
        }

        _newCategory = false;
        _dto = reply.Data;

        SetActionMessage( true, $"Successfully added {_dto.Type} category." );
        StateHasChanged();
    }
    async Task SubmitEdit()
    {
        ApiReply<bool> reply = await AdminCategoryService.UpdateCategory( _dto );

        if ( !reply.Success )
        {
            SetActionMessage( false, $"Failed to update {_dto.Type} category! {reply.Message}" );
            return;
        }
        
        SetActionMessage( true, $"Successfully updated {_dto.Type} category." );
        StateHasChanged();
    }
}