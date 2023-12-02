using Blazored.LocalStorage;
using BlazorElectronics.Shared;

namespace BlazorElectronics.Client.Services.Users.Admin.Features;

public class AdminFeaturedDealsServiceClient : AdminServiceClient, IAdminFeaturedDealsServiceClient
{
    const string API_ROUTE = "api/adminfeatures";
    const string API_ROUTE_GET_VIEW = API_ROUTE + "/get-deals-view";
    const string API_ROUTE_GET_EDIT = API_ROUTE + "/get-deal-edit";
    const string API_ROUTE_ADD = API_ROUTE + "/add-deal";
    const string API_ROUTE_UPDATE = API_ROUTE + "/update-deal";
    const string API_ROUTE_REMOVE = API_ROUTE + "/remove-deal";
    
    public AdminFeaturedDealsServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }
    
    public Task<ServiceReply<List<AdminItemViewDto>?>> GetView()
    {
        throw new NotImplementedException();
    }
    public Task<ServiceReply<bool>> RemoveItem( IntDto itemId )
    {
        throw new NotImplementedException();
    }
}