using System.Collections.Specialized;
using System.Web;
using BlazorElectronics.Client.Services.Admin;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin.Categories;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.UserViews.Admin;

public sealed partial class AdminCategoriesEdit
{
    [Inject] IAdminCategoryServiceClient AdminCategoryService { get; init; } = default!;
    
    bool _newCategory;
    int _categoryId = -1;
    int _categoryTier = -1;

    AddUpdateCategoryDto _dto = new();
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if ( !PageIsAuthorized )
        {
            Logger.LogError( "Not authorized!" );
            StartPageRedirection();
            return;
        }

        if ( !TryParseUrlParameters() )
        {
            RazorViewMessage = ERROR_INVALID_URL_PARAMS;
            Logger.LogError( ERROR_INVALID_URL_PARAMS );
            StartPageRedirection();
            return;
        }

        if ( _newCategory )
        {
            PageIsLoaded = true;
            _dto.Tier = _categoryTier;
            return;
        }

        var request = new GetCategoryEditRequest( _categoryId, _categoryTier );
        ApiReply<AddUpdateCategoryDto?> reply = await AdminCategoryService.GetCategoryEdit( request );

        if ( !reply.Success || reply.Data is null )
        {
            Logger.LogError( reply.Message ??= "Failed to get category!" );
            RazorViewMessage = reply.Message ??= "Failed to get category!";
            StartPageRedirection();
            return;
        }
        
        PageIsLoaded = true;
        _dto = reply.Data;
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
                      int.TryParse( categoryTierString, out _categoryTier ) &&
                      !string.IsNullOrWhiteSpace( newCategoryString ) &&
                      bool.TryParse( newCategoryString, out _newCategory );

        if ( !parsed )
            return false;

        if ( !_newCategory )
            parsed = !string.IsNullOrWhiteSpace( categoryIdString ) && int.TryParse( categoryIdString, out _categoryId );

        return parsed;
    }
    
    async Task<bool> SubmitNew()
    {
        ApiReply<AddUpdateCategoryDto?> reply = await AdminCategoryService.AddCategory( _dto );

        if ( !reply.Success || reply.Data is null )
        {
            RazorViewMessage = reply.Message ??= "Failed to add category!";
            return false;
        }

        _newCategory = false;
        _dto = reply.Data;
        
        StateHasChanged();
        return true;
    }
    async Task<bool> SubmitEdit()
    {
        ApiReply<bool> reply = await AdminCategoryService.UpdateCategory( _dto );

        if ( reply.Success ) 
            return true;
        
        RazorViewMessage = reply.Message ??= "Failed to submit changes!";
        return false;

    }
}