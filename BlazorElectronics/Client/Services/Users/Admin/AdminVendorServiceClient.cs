using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Vendors;

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
    
    public async Task<ServiceReply<VendorsViewDto?>> GetView()
    {
        return await TryUserRequest<VendorsViewDto?>( API_ROUTE_GET_VIEW );
    }
    public async Task<ServiceReply<VendorEditDto?>> GetEdit( IntDto dto )
    {
        return await TryUserRequest<IntDto, VendorEditDto?>( API_ROUTE_GET_EDIT, dto );
    }
    public async Task<ServiceReply<int>> Add( VendorEditDto dto )
    {
        return await TryUserRequest<VendorEditDto, int>( API_ROUTE_ADD, dto );
    }
    public async Task<ServiceReply<bool>> Update( VendorEditDto dto )
    {
        return await TryUserRequest<VendorEditDto, bool>( API_ROUTE_UPDATE, dto );
    }
    public async Task<ServiceReply<bool>> Remove( IntDto dto )
    {
        return await TryUserRequest<IntDto, bool>( API_ROUTE_REMOVE, dto );
    }
}