using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin.Categories;

namespace BlazorElectronics.Client.Services.Users.Admin;

public sealed class AdminCategoryServiceClient : AdminServiceClient, IAdminCategoryServiceClient
{
    const string API_ROUTE = "api/admincategory";
    const string API_ROUTE_GET_VIEW = API_ROUTE + "/get-categories-view";
    const string API_ROUTE_GET_EDIT = API_ROUTE + "/get-category-edit";
    const string API_ROUTE_ADD = API_ROUTE + "/add-category";
    const string API_ROUTE_UPDATE = API_ROUTE + "/update-category";
    const string API_ROUTE_REMOVE = API_ROUTE + "/remove-category";
    
    public AdminCategoryServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }

    public async Task<ApiReply<CategoryViewDto?>> GetCategoriesView()
    {
        return await TryUserApiRequest<CategoryViewDto?>( API_ROUTE_GET_VIEW );
    }
    public async Task<ApiReply<EditCategoryDto?>> GetCategoryEdit( GetCategoryEditDto data )
    {
        return await TryUserApiRequest<EditCategoryDto?>( API_ROUTE_GET_EDIT, data );
    }
    public async Task<ApiReply<EditCategoryDto?>> AddCategory( AddCategoryDto data )
    {
        return await TryUserApiRequest<EditCategoryDto?>( API_ROUTE_ADD, data );
    }
    public async Task<ApiReply<bool>> UpdateCategory( EditCategoryDto data )
    {
        return await TryUserApiRequest<bool>( API_ROUTE_UPDATE, data );
    }
    public async Task<ApiReply<bool>> RemoveCategory( RemoveCategoryDto data )
    {
        return await TryUserApiRequest<bool>( API_ROUTE_REMOVE, data );
    }
}