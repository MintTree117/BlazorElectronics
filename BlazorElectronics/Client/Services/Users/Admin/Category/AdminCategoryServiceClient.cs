using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Categories;

namespace BlazorElectronics.Client.Services.Users.Admin.Category;

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

    public async Task<ServiceReply<List<CategoryViewDto>?>> GetView()
    {
        return await TryUserRequest<List<CategoryViewDto>?>( API_ROUTE_GET_VIEW );
    }
    public async Task<ServiceReply<CategoryEditDto?>> GetCategoryEdit( IntDto data )
    {
        return await TryUserRequest<IntDto,CategoryEditDto?>( API_ROUTE_GET_EDIT, data );
    }
    public async Task<ServiceReply<CategoryEditDto?>> AddCategory( CategoryEditDto data )
    {
        return await TryUserRequest<CategoryEditDto,CategoryEditDto?>( API_ROUTE_ADD, data );
    }
    public async Task<ServiceReply<bool>> UpdateCategory( CategoryEditDto data )
    {
        return await TryUserRequest<CategoryEditDto,bool>( API_ROUTE_UPDATE, data );
    }
    public async Task<ServiceReply<bool>> RemoveItem( IntDto itemId )
    {
        return await TryUserRequest<IntDto, bool>( API_ROUTE_REMOVE, itemId );
    }
}