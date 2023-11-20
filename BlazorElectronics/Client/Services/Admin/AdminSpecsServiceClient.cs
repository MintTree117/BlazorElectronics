using BlazorElectronics.Client.Services.Users;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin.Specs;
using BlazorElectronics.Shared.Admin.Specs.SpecsSingle;

namespace BlazorElectronics.Client.Services.Admin;

public sealed class AdminSpecsServiceClient : AdminDbServiceClient, IAdminSpecsServiceClient
{
    const string API_ROUTE = "api/adminspeclookup";
    const string API_ROUTE_GET = API_ROUTE + "/get-specs-view";
    const string API_ROUTE_ADD = API_ROUTE + "/add-spec-lookup";
    const string API_ROUTE_UPDATE = API_ROUTE + "/update-spec-lookup";
    const string API_ROUTE_REMOVE = API_ROUTE + "/remove-spec-lookup";
    
    public AdminSpecsServiceClient( ILogger<ClientService> logger, IUserServiceClient userService, HttpClient http )
        : base( logger, userService, http ) { }
    
    public async Task<ApiReply<SpecsViewDto?>> GetSpecsView()
    {
        return await TryExecuteApiQuery<object?, SpecsViewDto?>( API_ROUTE_GET, null );
    }
    public async Task<ApiReply<EditSpecLookupDto?>> AddSpecLookup( AddSpecLookupDto dto )
    {
        return await TryExecuteApiQuery<AddSpecLookupDto, EditSpecLookupDto?>( API_ROUTE_GET, dto );
    }
    public async Task<ApiReply<bool>> UpdateSpecLookup( EditSpecLookupDto dto )
    {
        throw new NotImplementedException();
    }
    public async Task<ApiReply<bool>> RemoveSpecLookup( RemoveSpecLookupDto dto )
    {
        throw new NotImplementedException();
    }
}