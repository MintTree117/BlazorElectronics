using BlazorElectronics.Client.Services.Users;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin.Categories;

namespace BlazorElectronics.Client.Services.Admin;

public class AdminCategoryServiceClient : AdminDbServiceClient, IAdminCategoryServiceClient
{
    const string API_ROUTE = "api/admincategory";
    const string API_ROUTE_GET = API_ROUTE + "/get-category-edit";
    const string API_ROUTE_ADD = API_ROUTE + "/add-category";
    const string API_ROUTE_UPDATE = API_ROUTE + "/update-category";
    const string API_ROUTE_REMOVE = API_ROUTE + "/remove-category";
    
    public AdminCategoryServiceClient( ILogger<ClientService> logger, IUserServiceClient userService, HttpClient http )
        : base( logger, userService, http ) { }
    
    public async Task<ApiReply<AddUpdateCategoryDto?>> GetCategoryEdit( GetCategoryEditRequest dto )
    {
        return await TryExecuteApiQuery<GetCategoryEditRequest, AddUpdateCategoryDto?>( API_ROUTE_GET, dto );
    }
    public async Task<ApiReply<AddUpdateCategoryDto?>> AddCategory( AddUpdateCategoryDto dto )
    {
        return await TryExecuteApiQuery<AddUpdateCategoryDto, AddUpdateCategoryDto?>( API_ROUTE_ADD, dto );
    }
    public async Task<ApiReply<bool>> UpdateCategory( AddUpdateCategoryDto dto )
    {
        return await TryExecuteApiTransaction( API_ROUTE_UPDATE, dto );
    }
    public async Task<ApiReply<bool>> RemoveCategory( DeleteCategoryDto dto )
    {
        return await TryExecuteApiTransaction( API_ROUTE_REMOVE, dto );
    }
}