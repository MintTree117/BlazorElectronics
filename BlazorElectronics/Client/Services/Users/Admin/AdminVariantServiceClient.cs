using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin.Variants;

namespace BlazorElectronics.Client.Services.Users.Admin;

public sealed class AdminVariantServiceClient : AdminServiceClient, IAdminVariantServiceClient
{
    const string API_ROUTE = "api/AdminVariant";
    const string API_ROUTE_GET_VIEW = API_ROUTE + "/get-variants-view";
    const string API_ROUTE_GET_EDIT = API_ROUTE + "/get-variant-edit";
    const string API_ROUTE_ADD = API_ROUTE + "/add-variant";
    const string API_ROUTE_UPDATE = API_ROUTE + "/update-variant";
    const string API_ROUTE_REMOVE = API_ROUTE + "/remove-variant";
    
    public AdminVariantServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }

    public async Task<ApiReply<List<VariantViewDto>?>> GetView()
    {
        return await TryUserRequest<List<VariantViewDto>?>( API_ROUTE_GET_VIEW );
    }
    public async Task<ApiReply<VariantEditDto?>> GetEdit( IdDto data )
    {
        return await TryUserRequest<IdDto, VariantEditDto?>( API_ROUTE_GET_EDIT, data );
    }
    public async Task<ApiReply<int>> Add( VariantAddDto data )
    {
        return await TryUserRequest<VariantAddDto, int>( API_ROUTE_ADD, data );
    }
    public async Task<ApiReply<bool>> Update( VariantEditDto data )
    {
        return await TryUserRequest<VariantEditDto, bool>( API_ROUTE_UPDATE, data );
    }
    public async Task<ApiReply<bool>> Remove( IdDto data )
    {
        return await TryUserRequest<IdDto, bool>( API_ROUTE_REMOVE, data );
    }
}