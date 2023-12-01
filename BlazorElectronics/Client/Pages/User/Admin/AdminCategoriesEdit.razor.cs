using System.Collections.Specialized;
using System.Web;
using BlazorElectronics.Client.Services.Users.Admin;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.User.Admin;

public sealed partial class AdminCategoriesEdit : AdminView
{
    [Inject] IAdminCategoryServiceClient AdminCategoryService { get; init; } = default!;
    
    bool _newCategory;
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

        if ( !TryParseUrlParameters( out CategoryType type, out int id ) )
        {
            SetViewMessage( false, ERROR_INVALID_URL_PARAMS );
            Logger.LogError( ERROR_INVALID_URL_PARAMS );
            StartPageRedirection();
            return;
        }

        if ( _newCategory )
        {
            PageIsLoaded = true;
            _dto.Type = type;
            return;
        }

        var request = new CategoryGetEditDto( type, id );
        ServiceReply<CategoryEditDto?> reply = await AdminCategoryService.GetCategoryEdit( request );

        if ( !reply.Success || reply.Data is null )
        {
            Logger.LogError( reply.Message ?? "Failed to get category!" );
            SetViewMessage( false, reply.Message ?? "Failed to get category!" );
            StartPageRedirection();
            return;
        }
        
        PageIsLoaded = true;
        _dto = reply.Data;
        StateHasChanged();
    }
    bool TryParseUrlParameters( out CategoryType categoryType, out int categoryId )
    {
        categoryType = CategoryType.PRIMARY;
        categoryId = -1;
        
        Uri uri = NavManager.ToAbsoluteUri( NavManager.Uri );
        NameValueCollection queryString = HttpUtility.ParseQueryString( uri.Query );

        string? newCategoryString = queryString.Get( "newCategory" );
        string? categoryIdString = queryString.Get( "categoryId" );
        string? categoryTierString = queryString.Get( "categoryTier" );

        bool parsed = !string.IsNullOrWhiteSpace( categoryTierString ) &&
                      Enum.TryParse( categoryTierString, out categoryType ) &&
                      !string.IsNullOrWhiteSpace( newCategoryString ) &&
                      bool.TryParse( newCategoryString, out _newCategory );

        if ( _newCategory )
            return parsed;
        
        return !string.IsNullOrWhiteSpace( categoryIdString ) && int.TryParse( categoryIdString, out categoryId );
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
        ServiceReply<CategoryEditDto?> reply = await AdminCategoryService.AddCategory( request );
        
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
        ServiceReply<bool> reply = await AdminCategoryService.UpdateCategory( _dto );

        if ( !reply.Success )
        {
            SetActionMessage( false, $"Failed to update {_dto.Type} category! {reply.Message}" );
            return;
        }
        
        SetActionMessage( true, $"Successfully updated {_dto.Type} category." );
        StateHasChanged();
    }
}