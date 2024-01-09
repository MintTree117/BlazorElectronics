using Blazored.LocalStorage;
using BlazorElectronics.Client.Services.Users;
using BlazorElectronics.Shared;

namespace BlazorElectronics.Client.Services.Admin;

public class AdminServiceClient : UserServiceClient, IAdminServiceClient
{
    const string API_ROUTE = "api/_admin";
    const string API_ROUTE_AUTHORIZE = API_ROUTE + "/authorize-admin";

    public AdminServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }

    public async Task<ServiceReply<bool>> AuthorizeAdmin()
    {
        return await TryUserGetRequest<bool>( API_ROUTE_AUTHORIZE );
    }

    protected static Dictionary<string, object> GetItemIdParam( int id )
    {
        return new Dictionary<string, object>
        {
            { "ItemId", id }
        };
    }
}