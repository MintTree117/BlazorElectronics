using System.Net.Http.Json;
using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Inbound.Users;
using BlazorElectronics.Shared.Outbound.Users;

namespace BlazorElectronics.Client.Services.Users;

public class UserServiceClient : IUserServiceClient
{
    public event Action<bool>? SessionChanged;
    public event Action<string>? SessionStorageError;
    
    const string API_PATH_CONTROLLER = "api/UserAccount";
    const string API_PATH_REGISTER = API_PATH_CONTROLLER + "/register";
    const string API_PATH_LOGIN = API_PATH_CONTROLLER + "/login";
    const string API_PATH_LOGOUT = API_PATH_CONTROLLER + "/logout";
    const string API_PATH_AUTHORIZE = API_PATH_CONTROLLER + "/authorize";
    const string API_PATH_CHANGE_PASSWORD = API_PATH_CONTROLLER + "/change-password";
    const string SESSION_DATA_KEY = "UserSession";

    readonly ILogger<UserServiceClient> _logger;
    readonly HttpClient _http;
    readonly ILocalStorageService _localStorage;
    UserLoginResponse? _userSession;
    
    public UserServiceClient( ILogger<UserServiceClient> logger, HttpClient http, ILocalStorageService localStorage )
    {
        _logger = logger;
        _http = http;
        _localStorage = localStorage;
    }
    
    public async Task<ApiReply<UserLoginResponse?>> Register( UserRegisterRequest request )
    {
        ApiReply<UserLoginResponse?> registerReply = await TryGetLoginResponse( API_PATH_REGISTER, request );

        if ( !registerReply.Success || registerReply.Data is null )
            SessionChanged?.Invoke( true );
        
        return registerReply;
    }
    public async Task<ApiReply<UserLoginResponse?>> Login( UserLoginRequest request )
    {
        ApiReply<UserLoginResponse?> loginReply = await TryGetLoginResponse( API_PATH_LOGIN, request );

        if ( loginReply is { Success: true, Data: not null } )
            SessionChanged?.Invoke( true );

        return loginReply;
    }
    public async Task<ApiReply<bool>> Logout()
    {
        ApiReply<bool> serverReply = await TryExecuteApiRequest( API_PATH_LOGOUT );
        await _localStorage.RemoveItemAsync( SESSION_DATA_KEY );
        SessionChanged?.Invoke( false );
        return serverReply;
    }
    public async Task<ApiReply<bool>> AuthorizeUser()
    {
        return await TryExecuteApiRequest( API_PATH_AUTHORIZE );
    }
    public async Task<ApiReply<bool>> ChangePassword( UserChangePasswordRequest request )
    {
        return await TryExecuteApiRequest( API_PATH_CHANGE_PASSWORD );
    }
    
    async Task<ApiReply<UserLoginResponse?>> TryGetLoginResponse( string apiPath, object requestObject )
    {
        ApiReply<UserLoginResponse?>? loginReply;

        try
        {
            HttpResponseMessage httpResponse = await _http.PostAsJsonAsync( apiPath, requestObject );
            loginReply = await httpResponse.Content.ReadFromJsonAsync<ApiReply<UserLoginResponse?>?>();
        }
        catch ( Exception e )
        {
            return new ApiReply<UserLoginResponse?>( e.Message + e.InnerException );
        }

        if ( loginReply is null || !loginReply.Success || loginReply.Data is null )
            return new ApiReply<UserLoginResponse?>( loginReply?.Message );

        _userSession = loginReply.Data;
        
        try
        {
            await _localStorage.SetItemAsync( SESSION_DATA_KEY, _userSession );
        }
        catch ( Exception e )
        {
            SessionStorageError?.Invoke( e.Message + e.InnerException );
        }

        return new ApiReply<UserLoginResponse?>( _userSession );
    }
    async Task<ApiReply<bool>> TryExecuteApiRequest( string apiPath )
    {
        ApiReply<UserLoginResponse?> sessionReply = await TryGetLocalUserSession();

        if ( !sessionReply.Success || sessionReply.Data is null )
        {
            _logger.LogError( "Failed to get local session!" );
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
            _logger.LogError( "Failed to read POST response!" );
            return new ApiReply<bool>( e.Message );
        }
        
        _logger.LogError( apiReply.Data.ToString() + " " + apiReply.Success );

        return apiReply is not null && apiReply.Success
            ? apiReply
            : new ApiReply<bool>( apiReply?.Message );
    }
    async Task<ApiReply<UserLoginResponse?>> TryGetLocalUserSession()
    {
        if ( _userSession is not null )
            return new ApiReply<UserLoginResponse?>( _userSession );
        
        var storedSession = await _localStorage.GetItemAsync<UserLoginResponse?>( SESSION_DATA_KEY );

        if ( storedSession is null )
            return new ApiReply<UserLoginResponse?>( "Stored session is null!" );

        return storedSession.ValidateIntegrity( out string message )
            ? new ApiReply<UserLoginResponse?>( storedSession )
            : new ApiReply<UserLoginResponse?>( message );
    }
}