using Blazored.LocalStorage;
using BlazorElectronics.Shared;

namespace BlazorElectronics.Client.Services.Users.Admin;

public class AdminServiceClient : UserServiceClient, IAdminServiceClient
{
    const string API_ROUTE = "api/_admin";
    const string API_ROUTE_AUTHORIZE = API_ROUTE + "/authorize";

    public AdminServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }

    public async Task<ServiceReply<bool>> AuthorizeAdmin()
    {
        return await TryUserRequest<bool>( API_ROUTE_AUTHORIZE );
    }
}