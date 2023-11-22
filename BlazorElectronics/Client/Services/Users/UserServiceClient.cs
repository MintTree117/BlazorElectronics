using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Inbound.Users;
using BlazorElectronics.Shared.Outbound.Users;

namespace BlazorElectronics.Client.Services.Users;

public class UserServiceClient : ClientService, IUserServiceClient
{
    public UserServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }

    public event Action<UserSessionResponse?>? SessionChanged;
    public event Action<string>? SessionStorageError;

    const string API_ROUTE = "api/UserAccount";
    const string API_ROUTE_REGISTER = API_ROUTE + "/register";
    const string API_ROUTE_LOGIN = API_ROUTE + "/login";
    const string API_ROUTE_LOGOUT = API_ROUTE + "/logout";
    const string API_ROUTE_AUTHORIZE = API_ROUTE + "/authorize";
    const string API_ROUTE_CHANGE_PASSWORD = API_ROUTE + "/change-password";
    const string SESSION_DATA_KEY = "UserSession";
    
    UserSessionResponse? _userSession;
    
    public async Task<ApiReply<UserSessionResponse?>> Register( UserRegisterRequest request )
    {
        ApiReply<UserSessionResponse?> registerReply = await TryGetSessionResponse( API_ROUTE_REGISTER, request );

        if ( !registerReply.Success || registerReply.Data is null )
            SessionChanged?.Invoke( _userSession );
        
        return registerReply;
    }
    public async Task<ApiReply<UserSessionResponse?>> Login( UserLoginRequest request )
    {
        ApiReply<UserSessionResponse?> loginReply = await TryGetSessionResponse( API_ROUTE_LOGIN, request );

        if ( loginReply is { Success: true, Data: not null } )
            SessionChanged?.Invoke( _userSession );

        return loginReply;
    }
    public async Task<ApiReply<UserSessionResponse?>> AuthorizeUser()
    {
        ApiReply<bool> apiReply = await TryUserRequest<bool>( API_ROUTE_AUTHORIZE );

        return apiReply.Success
            ? new ApiReply<UserSessionResponse?>( _userSession )
            : new ApiReply<UserSessionResponse?>( apiReply.Message );
    }
    public async Task<ApiReply<bool>> Logout()
    {
        ApiReply<bool> serverReply = await TryUserRequest<bool>( API_ROUTE_LOGOUT );
        await Storage.RemoveItemAsync( SESSION_DATA_KEY );
        SessionChanged?.Invoke( null );
        return serverReply;
    }
    public async Task<ApiReply<bool>> ChangePassword( PasswordChangeRequest request )
    {
        return await TryUserRequest<PasswordChangeRequest, bool>( API_ROUTE_CHANGE_PASSWORD, request );
    }
    public async Task<ApiReply<UserSessionResponse?>> TryGetLocalUserSession()
    {
        if ( _userSession is not null )
            return new ApiReply<UserSessionResponse?>( _userSession );

        UserSessionResponse? session;

        try
        {
            session = await Storage.GetItemAsync<UserSessionResponse?>( SESSION_DATA_KEY );
        }
        catch ( Exception e )
        {
            Logger.LogError( e.Message + e.InnerException?.Message );
            return new ApiReply<UserSessionResponse?>( e.Message );
        }

        if ( session is null )
            return new ApiReply<UserSessionResponse?>( "Stored session is null!" );

        if ( !session.ValidateIntegrity( out string message ) )
            return new ApiReply<UserSessionResponse?>( message );

        _userSession = session;
        return new ApiReply<UserSessionResponse?>( _userSession );
    }

    protected async Task<ApiReply<T?>> TryUserRequest<T>( string apiRoute )
    {
        ApiReply<UserSessionResponse?> sessionReply = await TryGetLocalUserSession();

        if ( !sessionReply.Success || sessionReply.Data is null )
        {
            Logger.LogError( sessionReply.Message );
            return new ApiReply<T?>( sessionReply.Message );
        }

        var apiRequest = new UserRequest( sessionReply.Data );
        return await TryPostRequest<T?>( apiRoute, apiRequest );
    }
    protected async Task<ApiReply<RESPONSE?>> TryUserRequest<REQUEST,RESPONSE>( string apiRoute, REQUEST payload ) where REQUEST : class
    {
        ApiReply<UserSessionResponse?> sessionReply = await TryGetLocalUserSession();

        if ( !sessionReply.Success || sessionReply.Data is null )
        {
            Logger.LogError( sessionReply.Message );
            return new ApiReply<RESPONSE?>( sessionReply.Message );
        }

        var apiRequest = new UserDataRequest<REQUEST>( sessionReply.Data, payload );
        return await TryPostRequest<RESPONSE?>( apiRoute, apiRequest );
    }
    async Task<ApiReply<UserSessionResponse?>> TryGetSessionResponse<T>( string apiPath, T requestObject )
    {
        ApiReply<UserSessionResponse?> sessionReply = await TryPostRequest<UserSessionResponse?>( apiPath, requestObject );

        if ( !sessionReply.Success || sessionReply.Data is null )
            return sessionReply;
        
        _userSession = sessionReply.Data;

        await TryStoreSessionResponse();
        
        return new ApiReply<UserSessionResponse?>( _userSession );
    }
    async Task TryStoreSessionResponse()
    {
        try
        {
            await Storage.SetItemAsync( SESSION_DATA_KEY, _userSession );
        }
        catch ( Exception e )
        {
            Logger.LogError( e.Message + e.InnerException?.Message );
        }
    }
}