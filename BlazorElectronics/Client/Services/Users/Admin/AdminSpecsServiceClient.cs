using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin.SpecLookups;
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
    
    public async Task<ApiReply<List<SpecLookupViewDto>?>> GetView()
    {
        return await TryUserRequest<List<SpecLookupViewDto>?>( API_ROUTE_GET_VIEW );
    }
    public async Task<ApiReply<SpecLookupEditDto?>> GetEdit( IdDto data )
    {
        return await TryUserRequest<IdDto,SpecLookupEditDto?>( API_ROUTE_GET_EDIT, data );
    }
    public async Task<ApiReply<int>> Add( SpecLookupEditDto data )
    {
        return await TryUserRequest<SpecLookupEditDto,int>( API_ROUTE_ADD, data );
    }
    public async Task<ApiReply<bool>> Update( SpecLookupEditDto data )
    {
        return await TryUserRequest<SpecLookupEditDto,bool>( API_ROUTE_UPDATE, data );
    }
    public async Task<ApiReply<bool>> Remove( IdDto data )
    {
        return await TryUserRequest<IdDto,bool>( API_ROUTE_REMOVE, data );
    }
}