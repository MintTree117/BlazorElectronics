using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Categories;

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

    public async Task<ServiceReply<CategoriesViewDto?>> GetCategoriesView()
    {
        return await TryUserRequest<CategoriesViewDto?>( API_ROUTE_GET_VIEW );
    }
    public async Task<ServiceReply<CategoryEditDto?>> GetCategoryEdit( CategoryGetEditDto data )
    {
        return await TryUserRequest<CategoryGetEditDto,CategoryEditDto?>( API_ROUTE_GET_EDIT, data );
    }
    public async Task<ServiceReply<CategoryEditDto?>> AddCategory( CategoryAddDto data )
    {
        return await TryUserRequest<CategoryAddDto,CategoryEditDto?>( API_ROUTE_ADD, data );
    }
    public async Task<ServiceReply<bool>> UpdateCategory( CategoryEditDto data )
    {
        return await TryUserRequest<CategoryEditDto,bool>( API_ROUTE_UPDATE, data );
    }
    public async Task<ServiceReply<bool>> RemoveCategory( CategoryRemoveDto data )
    {
        return await TryUserRequest<CategoryRemoveDto,bool>( API_ROUTE_REMOVE, data );
    }
}