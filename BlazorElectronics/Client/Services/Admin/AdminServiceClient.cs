using System.Net.Http.Json;
using BlazorElectronics.Client.Services.Users;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Outbound.Users;

namespace BlazorElectronics.Client.Services.Admin;

public class AdminServiceClient : ClientService<AdminServiceClient>, IAdminServiceClient
{
    readonly HttpClient _http;
    readonly IUserServiceClient _userService;
    
    public AdminServiceClient( ILogger<AdminServiceClient> logger, IUserServiceClient userService, HttpClient http )
        : base( logger )
    {
        _userService = userService;
        _http = http;
    }
    
    public async Task<ApiReply<bool>> AuthorizeAdminView()
    {
        ApiReply<UserSessionResponse?> localReply = await _userService.TryGetLocalUserSession();

        if ( !localReply.Success || localReply.Data is null )
            return new ApiReply<bool>( localReply.Message );

        ApiReply<bool>? authorizeReply;
        
        try
        {
            HttpResponseMessage httpResponse = await _http.PostAsJsonAsync( "authorize-admin", localReply.Data );
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