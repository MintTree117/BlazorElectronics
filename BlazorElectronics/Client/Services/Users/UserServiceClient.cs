using System.Net.Http.Json;
using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Inbound.Users;
using BlazorElectronics.Shared.Outbound.Users;

namespace BlazorElectronics.Client.Services.Users;

public class UserServiceClient : ClientService, IUserServiceClient
{
    public UserServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService localStorage )
        : base( logger )
    {
        _http = http;
        _localStorage = localStorage;
    }
    
    public event Action<UserSessionResponse?>? SessionChanged;
    public event Action<string>? SessionStorageError;
    
    const string API_PATH_CONTROLLER = "api/UserAccount";
    const string API_PATH_REGISTER = API_PATH_CONTROLLER + "/register";
    const string API_PATH_LOGIN = API_PATH_CONTROLLER + "/login";
    const string API_PATH_LOGOUT = API_PATH_CONTROLLER + "/logout";
    const string API_PATH_AUTHORIZE = API_PATH_CONTROLLER + "/authorize";
    const string API_PATH_CHANGE_PASSWORD = API_PATH_CONTROLLER + "/change-password";
    const string SESSION_DATA_KEY = "UserSession";

    readonly HttpClient _http;
    readonly ILocalStorageService _localStorage;
    UserSessionResponse? _userSession;

    public async Task<ApiReply<UserSessionResponse?>> Register( UserRegisterRequest request )
    {
        ApiReply<UserSessionResponse?> registerReply = await TryGetLoginResponse( API_PATH_REGISTER, request );

        if ( !registerReply.Success || registerReply.Data is null )
            SessionChanged?.Invoke( _userSession );
        
        return registerReply;
    }
    public async Task<ApiReply<UserSessionResponse?>> Login( UserLoginRequest request )
    {
        ApiReply<UserSessionResponse?> loginReply = await TryGetLoginResponse( API_PATH_LOGIN, request );

        if ( loginReply is { Success: true, Data: not null } )
            SessionChanged?.Invoke( _userSession );

        return loginReply;
    }
    public async Task<ApiReply<UserSessionResponse?>> AuthorizeUser()
    {
        ApiReply<UserSessionResponse?> localSessionReply = await TryGetLocalUserSession();

        if ( !localSessionReply.Success || _userSession is null )
            return new ApiReply<UserSessionResponse?>( localSessionReply.Message );

        ApiReply<bool>? authorizeReply;
        
        try
        {
            HttpResponseMessage httpResponse = await _http.PostAsJsonAsync( API_PATH_AUTHORIZE, _userSession );
            authorizeReply = await httpResponse.Content.ReadFromJsonAsync<ApiReply<bool>?>();
        }
        catch ( Exception e )
        {
            Logger.LogError( e.Message + e.InnerException );
            return new ApiReply<UserSessionResponse?>( e.Message + e.InnerException );
        }

        if ( authorizeReply is null || !authorizeReply.Success )
            return new ApiReply<UserSessionResponse?>( authorizeReply?.Message );
        
        return authorizeReply.Success
            ? new ApiReply<UserSessionResponse?>( _userSession )
            : new ApiReply<UserSessionResponse?>( authorizeReply.Message );
    }
    public async Task<ApiReply<bool>> AuthorizeAdmin()
    {
        ApiReply<UserSessionResponse?> authorizeUserReply = await AuthorizeUser();

        if ( !authorizeUserReply.Success || authorizeUserReply.Data is null )
            return new ApiReply<bool>( authorizeUserReply.Message );

        return authorizeUserReply.Data.IsAdmin
            ? new ApiReply<bool>( true )
            : new ApiReply<bool>( "You are not authorized to to do this!!!" );
    }
    public async Task<ApiReply<bool>> Logout()
    {
        ApiReply<bool> serverReply = await TryExecuteApiRequest( API_PATH_LOGOUT );
        await _localStorage.RemoveItemAsync( SESSION_DATA_KEY );
        SessionChanged?.Invoke( null );
        return serverReply;
    }
    public async Task<ApiReply<bool>> ChangePassword( UserChangePasswordRequest request )
    {
        return await TryExecuteApiRequest( API_PATH_CHANGE_PASSWORD );
    }
    public async Task<ApiReply<UserSessionResponse?>> TryGetLocalUserSession()
    {
        if ( _userSession is not null )
            return new ApiReply<UserSessionResponse?>( _userSession );

        var storedSession = await _localStorage.GetItemAsync<UserSessionResponse?>( SESSION_DATA_KEY );

        if ( storedSession is null )
            return new ApiReply<UserSessionResponse?>( "Stored session is null!" );

        if ( !storedSession.ValidateIntegrity( out string message ) )
            return new ApiReply<UserSessionResponse?>( message );

        _userSession = storedSession;
        return new ApiReply<UserSessionResponse?>( _userSession );
    }
    
    async Task<ApiReply<UserSessionResponse?>> TryGetLoginResponse( string apiPath, object requestObject )
    {
        ApiReply<UserSessionResponse?>? sessionReply;

        try
        {
            HttpResponseMessage httpResponse = await _http.PostAsJsonAsync( apiPath, requestObject );
            sessionReply = await httpResponse.Content.ReadFromJsonAsync<ApiReply<UserSessionResponse?>?>();
        }
        catch ( Exception e )
        {
            Logger.LogError( e.Message + e.InnerException );
            return new ApiReply<UserSessionResponse?>( e.Message + e.InnerException );
        }

        if ( sessionReply is null || !sessionReply.Success || sessionReply.Data is null )
            return new ApiReply<UserSessionResponse?>( sessionReply?.Message );

        _userSession = sessionReply.Data;
        
        try
        {
            await _localStorage.SetItemAsync( SESSION_DATA_KEY, _userSession );
        }
        catch ( Exception e )
        {
            SessionStorageError?.Invoke( e.Message + e.InnerException );
        }

        return new ApiReply<UserSessionResponse?>( _userSession );
    }
    async Task<ApiReply<bool>> TryExecuteApiRequest( string apiPath )
    {
        ApiReply<UserSessionResponse?> sessionReply = await TryGetLocalUserSession();

        if ( !sessionReply.Success || sessionReply.Data is null )
        {
            Logger.LogError( "Failed to get local session!" );
            return new ApiReply<bool>( sessionReply.Message );   
        }

        var apiRequest = new SessionApiRequest( sessionReply.Data.SessionId, sessionReply.Data.SessionToken );
        ApiReply<bool>? apiReply;
        
        try
        {
            HttpResponseMessage httpResponse = await _http.PostAsJsonAsync( apiPath, apiRequest );
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