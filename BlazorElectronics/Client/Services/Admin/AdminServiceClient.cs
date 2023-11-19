using System.Net.Http.Json;
using BlazorElectronics.Client.Services.Users;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Outbound.Users;

namespace BlazorElectronics.Client.Services.Admin;

public class AdminServiceClient : ClientService, IAdminServiceClient
{
    const string API_ROUTE = "api/_admin";
    const string API_ROUTE_AUTHORIZE = API_ROUTE + "/authorize";
    
    protected readonly HttpClient _http;
    protected readonly IUserServiceClient UserService;
    
    public AdminServiceClient( ILogger<ClientService> logger, IUserServiceClient userService, HttpClient http )
        : base( logger )
    {
        UserService = userService;
        _http = http;
    }
    
    public async Task<ApiReply<bool>> AuthorizeAdmin()
    {
        ApiReply<UserSessionResponse?> localReply = await UserService.TryGetLocalUserSession();

        if ( !localReply.Success || localReply.Data is null )
            return new ApiReply<bool>( localReply.Message );

        ApiReply<bool>? authorizeReply;
        
        try
        {
            HttpResponseMessage httpResponse = await _http.PostAsJsonAsync( API_ROUTE_AUTHORIZE, localReply.Data );
            authorizeReply = await httpResponse.Content.ReadFromJsonAsync<ApiReply<bool>?>();
        }
        catch ( Exception e )
        {
            Logger.LogError( e.Message + e.InnerException );
            return new ApiReply<bool>( e.Message + e.InnerException );
        }

        return authorizeReply is not null && authorizeReply.Success
            ? new ApiReply<bool>( true )
            : new ApiReply<bool>( authorizeReply?.Message );
    }
}