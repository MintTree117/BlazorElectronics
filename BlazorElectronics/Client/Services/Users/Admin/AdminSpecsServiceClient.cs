using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.SpecLookups;

namespace BlazorElectronics.Client.Services.Users.Admin;

public sealed class AdminSpecsServiceClient : AdminServiceClient, IAdminSpecsServiceClient
{
    const string API_ROUTE = "api/adminspeclookup";
    const string API_ROUTE_GET_VIEW = API_ROUTE + "/get-spec-lookup-view";
    const string API_ROUTE_GET_EDIT = API_ROUTE + "/get-spec-lookup-edit";
    const string API_ROUTE_ADD = API_ROUTE + "/add-spec-lookup";
    const string API_ROUTE_UPDATE = API_ROUTE + "/update-spec-lookup";
    const string API_ROUTE_REMOVE = API_ROUTE + "/remove-spec-lookup";
    
    public AdminSpecsServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }
    
    public async Task<ServiceReply<List<SpecLookupViewDto>?>> GetView()
    {
        return await TryUserRequest<List<SpecLookupViewDto>?>( API_ROUTE_GET_VIEW );
    }
    public async Task<ServiceReply<SpecLookupEditDto?>> GetEdit( IntDto data )
    {
        return await TryUserRequest<IntDto,SpecLookupEditDto?>( API_ROUTE_GET_EDIT, data );
    }
    public async Task<ServiceReply<int>> Add( SpecLookupEditDto data )
    {
        return await TryUserRequest<SpecLookupEditDto,int>( API_ROUTE_ADD, data );
    }
    public async Task<ServiceReply<bool>> Update( SpecLookupEditDto data )
    {
        return await TryUserRequest<SpecLookupEditDto,bool>( API_ROUTE_UPDATE, data );
    }
    public async Task<ServiceReply<bool>> Remove( IntDto data )
    {
        return await TryUserRequest<IntDto,bool>( API_ROUTE_REMOVE, data );
    }
}