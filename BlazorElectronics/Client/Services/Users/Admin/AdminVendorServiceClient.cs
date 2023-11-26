using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin.Vendors;

namespace BlazorElectronics.Client.Services.Users.Admin;

public class AdminVendorServiceClient : AdminServiceClient, IAdminVendorServiceClient
{
    const string API_ROUTE = "api/adminvendor";
    const string API_ROUTE_GET_VIEW = API_ROUTE + "/get-vendors-view";
    const string API_ROUTE_GET_EDIT = API_ROUTE + "/get-vendor-edit";
    const string API_ROUTE_ADD = API_ROUTE + "/add-vendor";
    const string API_ROUTE_UPDATE = API_ROUTE + "/update-vendor";
    const string API_ROUTE_REMOVE = API_ROUTE + "/remove-vendor";
    
    public AdminVendorServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }
    
    public async Task<ApiReply<VendorsViewDto?>> GetView()
    {
        return await TryUserRequest<VendorsViewDto?>( API_ROUTE_GET_VIEW );
    }
    public async Task<ApiReply<VendorEditDto?>> GetEdit( IdDto dto )
    {
        return await TryUserRequest<IdDto, VendorEditDto?>( API_ROUTE_GET_EDIT, dto );
    }
    public async Task<ApiReply<int>> Add( VendorEditDto dto )
    {
        return await TryUserRequest<VendorEditDto, int>( API_ROUTE_ADD, dto );
    }
    public async Task<ApiReply<bool>> Update( VendorEditDto dto )
    {
        return await TryUserRequest<VendorEditDto, bool>( API_ROUTE_UPDATE, dto );
    }
    public async Task<ApiReply<bool>> Remove( IdDto dto )
    {
        return await TryUserRequest<IdDto, bool>( API_ROUTE_REMOVE, dto );
    }
}