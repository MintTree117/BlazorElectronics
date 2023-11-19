using System.Collections.Specialized;
using System.Web;
using BlazorElectronics.Client.Services.Admin;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin.Categories;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.Admin;

public partial class AdminCategoriesEdit
{
    [Inject] IAdminCategoryServiceClient AdminCategoryService { get; init; } = default!;
    
    bool _newCategory;
    int _categoryId = -1;
    int _categoryTier = -1;

    AddUpdateCategoryDto _dto = new();
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if ( !IsAuthorized )
        {
            Logger.LogError( "Not authorized!" );
            await HandleRedirection();
            return;
        }

        if ( !TryParseUrlParameters() )
        {
            Message = INVALID_QUERY_PARAMS_MESSAGE;
            Logger.LogError( INVALID_QUERY_PARAMS_MESSAGE );
            await HandleRedirection();
            return;
        }

        if ( _newCategory )
        {
            _dto.Tier = _categoryTier;
            return;
        }

        var request = new GetCategoryEditRequest( _categoryId, _categoryTier );
        ApiReply<AddUpdateCategoryDto?> reply = await AdminCategoryService.GetCategoryEdit( request );

        if ( !reply.Success || reply.Data is null )
        {
            Message = reply.Message ??= "Failed to get category!";
            await HandleRedirection();
            return;
        }

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
            Message = reply.Message ??= "Failed to add category!";
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
        
        Message = reply.Message ??= "Failed to submit changes!";
        return false;

    }
}