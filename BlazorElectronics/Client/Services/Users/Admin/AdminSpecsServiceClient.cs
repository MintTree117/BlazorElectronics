using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin.Specs;

namespace BlazorElectronics.Client.Services.Users.Admin;

public sealed class AdminSpecsServiceClient : AdminServiceClient, IAdminSpecsServiceClient
{
    const string API_ROUTE = "api/adminspeclookup";
    const string API_ROUTE_GET_VIEW = API_ROUTE + "/get-specs-lookup-view";
    const string API_ROUTE_GET_EDIT = API_ROUTE + "/get-spec-lookup-edit";
    const string API_ROUTE_ADD = API_ROUTE + "/add-spec-lookup";
    const string API_ROUTE_UPDATE = API_ROUTE + "/update-spec-lookup";
    const string API_ROUTE_REMOVE = API_ROUTE + "/remove-spec-lookup";
    
    public AdminSpecsServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }
    
    public async Task<ApiReply<SpecsViewDto?>> GetView()
    {
        return await TryUserApiRequest<SpecsViewDto?>( API_ROUTE_GET_VIEW );
    }
    public async Task<ApiReply<EditSpecLookupDto?>> GetEdit( GetSpecLookupEditDto data )
    {
        return await TryUserApiRequest<EditSpecLookupDto?>( API_ROUTE_GET_EDIT, data );
    }
    public async Task<ApiReply<EditSpecLookupDto?>> Add( AddSpecLookupDto data )
    {
        return await TryUserApiRequest<EditSpecLookupDto?>( API_ROUTE_ADD, data );
    }
    public async Task<ApiReply<bool>> Update( EditSpecLookupDto data )
    {
        return await TryUserApiRequest<bool>( API_ROUTE_UPDATE, data );
    }
    public async Task<ApiReply<bool>> Remove( RemoveSpecLookupDto data )
    {
        return await TryUserApiRequest<bool>( API_ROUTE_REMOVE, data );
    }
}