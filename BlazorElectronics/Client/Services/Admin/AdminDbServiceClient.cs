using System.Net.Http.Json;
using BlazorElectronics.Client.Services.Users;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin;
using BlazorElectronics.Shared.Inbound.Users;
using BlazorElectronics.Shared.Outbound.Users;

namespace BlazorElectronics.Client.Services.Admin;

public class AdminDbServiceClient : AdminServiceClient
{
    public AdminDbServiceClient( ILogger<ClientService> logger, IUserServiceClient userService, HttpClient http )
        : base( logger, userService, http ) { }

    protected async Task<ApiReply<T2?>> TryExecuteApiQuery<T1,T2>( string apiPath, T1 dto )
    {
        ApiReply<UserSessionResponse?> sessionReply = await UserService.TryGetLocalUserSession();

        if ( !sessionReply.Success || sessionReply.Data is null )
        {
            Logger.LogError( "Failed to get local session!" );
            return new ApiReply<T2?>( sessionReply.Message );
        }

        var sessionRequest = new SessionApiRequest( sessionReply.Data.SessionId, sessionReply.Data.SessionToken );
        var adminRequest = new AdminRequest<T1>( sessionRequest, dto );

        ApiReply<T2?>? apiReply;

        try
        {
            HttpResponseMessage httpResponse = await _http.PostAsJsonAsync( apiPath, adminRequest );
            apiReply = await httpResponse.Content.ReadFromJsonAsync<ApiReply<T2?>?>();
        }
        catch ( Exception e )
        {
            Logger.LogError( e.Message + e.InnerException );
            return new ApiReply<T2?>( e.Message );
        }

        return apiReply is { Success: true, Data: not null }
            ? apiReply
            : new ApiReply<T2?>( apiReply?.Message );
    }
    protected async Task<ApiReply<bool>> TryExecuteApiTransaction<T>( string apiPath, T dto )
    {
        ApiReply<UserSessionResponse?> sessionReply = await UserService.TryGetLocalUserSession();

        if ( !sessionReply.Success || sessionReply.Data is null )
        {
            Logger.LogError( "Failed to get local session!" );
            return new ApiReply<bool>( sessionReply.Message );
        }

        var sessionRequest = new SessionApiRequest( sessionReply.Data.SessionId, sessionReply.Data.SessionToken );
        var adminRequest = new AdminRequest<T>( sessionRequest, dto );
        
        ApiReply<bool>? apiReply;

        try
        {
            HttpResponseMessage httpResponse = await _http.PostAsJsonAsync( apiPath, adminRequest );
            apiReply = await httpResponse.Content.ReadFromJsonAsync<ApiReply<bool>?>();
        }
        catch ( Exception e )
        {
            Logger.LogError( e.Message + e.InnerException );
            return new ApiReply<bool>( e.Message );
        }

        return apiReply is not null && apiReply.Success
            ? apiReply
            : new ApiReply<bool>( apiReply?.Message );
    }
}