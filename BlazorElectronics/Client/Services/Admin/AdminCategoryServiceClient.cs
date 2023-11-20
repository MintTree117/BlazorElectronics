using BlazorElectronics.Client.Services.Users;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin.Categories;

namespace BlazorElectronics.Client.Services.Admin;

public sealed class AdminCategoryServiceClient : AdminDbServiceClient, IAdminCategoryServiceClient
{
    const string API_ROUTE = "api/admincategory";
    const string API_ROUTE_GET_VIEW = API_ROUTE + "/get-categories-view";
    const string API_ROUTE_GET_EDIT = API_ROUTE + "/get-category-edit";
    const string API_ROUTE_ADD = API_ROUTE + "/add-category";
    const string API_ROUTE_UPDATE = API_ROUTE + "/update-category";
    const string API_ROUTE_REMOVE = API_ROUTE + "/remove-category";
    
    public AdminCategoryServiceClient( ILogger<ClientService> logger, IUserServiceClient userService, HttpClient http )
        : base( logger, userService, http ) { }

    public async Task<ApiReply<CategoryViewDto?>> GetCategoriesView()
    {
        return await TryExecuteApiQuery<object?, CategoryViewDto?>( API_ROUTE_GET_VIEW, null );
    }
    public async Task<ApiReply<EditCategoryDto?>> GetCategoryEdit( GetCategoryEditDto request )
    {
        return await TryExecuteApiQuery<GetCategoryEditDto, EditCategoryDto?>( API_ROUTE_GET_EDIT, request );
    }
    public async Task<ApiReply<EditCategoryDto?>> AddCategory( AddCategoryDto dto )
    {
        return await TryExecuteApiQuery<AddCategoryDto, EditCategoryDto?>( API_ROUTE_ADD, dto );
    }
    public async Task<ApiReply<bool>> UpdateCategory( EditCategoryDto dto )
    {
        return await TryExecuteApiTransaction( API_ROUTE_UPDATE, dto );
    }
    public async Task<ApiReply<bool>> RemoveCategory( RemoveCategoryDto dto )
    {
        return await TryExecuteApiTransaction( API_ROUTE_REMOVE, dto );
    }
}