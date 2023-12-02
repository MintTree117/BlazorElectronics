using Blazored.LocalStorage;
using BlazorElectronics.Shared;

namespace BlazorElectronics.Client.Services.Users.Admin.Features;

public class AdminFeaturesServiceClient : AdminServiceClient, IAdminFeaturesServiceClient
{
    const string API_ROUTE = "api/adminfeatures";
    const string API_ROUTE_GET_VIEW = API_ROUTE + "/get-features-view";
    const string API_ROUTE_GET_EDIT = API_ROUTE + "/get-feature-edit";
    const string API_ROUTE_ADD = API_ROUTE + "/add-feature";
    const string API_ROUTE_UPDATE = API_ROUTE + "/update-feature";
    const string API_ROUTE_REMOVE = API_ROUTE + "/remove-feature";

    public AdminFeaturesServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }
    
    public async Task<ServiceReply<List<AdminItemViewDto>?>> GetView()
    {
        throw new NotImplementedException();
    }
    public async Task<ServiceReply<bool>> RemoveItem( IntDto itemId )
    {
        throw new NotImplementedException();
    }
}