using System.Net.Http.Json;
using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Inbound.Users;
using BlazorElectronics.Shared.Outbound.Users;

namespace BlazorElectronics.Client.Services.Users;

public class UserServiceClient : IUserServiceClient
{
    public event Action<bool>? AuthorizationChanged;

    const string API_PATH_CONTROLLER = "api/UserAccount";
    const string API_PATH_REGISTER = API_PATH_CONTROLLER + "/register";
    const string API_PATH_LOGIN = API_PATH_CONTROLLER + "/login";
    const string API_PATH_LOGOUT = API_PATH_CONTROLLER + "/logout";
    const string API_PATH_AUTHORIZE = API_PATH_CONTROLLER + "/authorize";
    const string API_PATH_CHANGE_PASSWORD = API_PATH_CONTROLLER + "/change-password";
    const string SESSION_DATA_KEY = "UserSession";
    
    readonly HttpClient _http;
    readonly ILocalStorageService _localStorage;

    public UserServiceClient( HttpClient http, ILocalStorageService localStorage )
    {
        _http = http;
        _localStorage = localStorage;
    }
    
    public async Task<ServiceResponse<UserLoginResponse?>> Register( UserRegisterRequest request )
    {
        try
        {
            ServiceResponse<UserSessionData?> localSession = await GetUserSession();

            if ( !localSession.Success )
                return new ServiceResponse<UserLoginResponse?>( null, false, localSession.Message! );

            request.ApiRequest = new SessionApiRequest( localSession.Data!.Username, localSession.Data.Token );
            
            HttpResponseMessage httpResponse = await _http.PostAsJsonAsync( API_PATH_REGISTER, request );
            var serviceResponse = await httpResponse.Content.ReadFromJsonAsync<ServiceResponse<UserLoginResponse>>();

            if ( serviceResponse == null )
                return new ServiceResponse<UserLoginResponse?>( null, false, "Service response is null!" );

            await _localStorage.SetItemAsync( SESSION_DATA_KEY, new UserSessionData( serviceResponse.Data!.Username, serviceResponse.Data.SessionToken ) );
            AuthorizationChanged?.Invoke( true );
            
            return (!serviceResponse.Success
                ? new ServiceResponse<UserLoginResponse?>( null, false, serviceResponse.Message ??= "Failed to retrieve Search Suggestions; message is null!" )!
                : serviceResponse)!;
        }
        catch ( Exception e )
        {
            return new ServiceResponse<UserLoginResponse?>( null, false, e.Message );
        }
    }
    public async Task<ServiceResponse<UserLoginResponse?>> Login( UserLoginRequest request )
    {
        try
        {
            HttpResponseMessage httpResponse = await _http.PostAsJsonAsync( API_PATH_LOGIN, request );
            var serviceResponse = await httpResponse.Content.ReadFromJsonAsync<ServiceResponse<UserLoginResponse?>>();

            if ( serviceResponse == null )
                return new ServiceResponse<UserLoginResponse?>( null, false, "Service response is null!" );

            await _localStorage.SetItemAsync( SESSION_DATA_KEY, new UserSessionData( serviceResponse.Data!.Username, serviceResponse.Data.SessionToken ) );
            AuthorizationChanged?.Invoke( true );
            
            return !serviceResponse.Success
                ? new ServiceResponse<UserLoginResponse?>( null, false, serviceResponse.Message ??= "Failed to login user; message is null!" )
                : serviceResponse;
        }
        catch ( Exception e )
        {
            return new ServiceResponse<UserLoginResponse?>( null, false, e.Message );
        }
    }
    public async Task<ServiceResponse<bool>> Logout()
    {
        var sessionData = await _localStorage.GetItemAsync<UserSessionData>( SESSION_DATA_KEY );
        await _localStorage.RemoveItemAsync( SESSION_DATA_KEY );
        AuthorizationChanged?.Invoke( false );

        try
        {
            HttpResponseMessage httpResponse = await _http.PostAsJsonAsync( API_PATH_LOGOUT, new SessionApiRequest( sessionData.Username, sessionData.Token ) );
            var logoutResponse = await httpResponse.Content.ReadFromJsonAsync<ServiceResponse<bool>>();

            return logoutResponse ?? new ServiceResponse<bool>( false, false, "Logout response is null!" );
        }
        catch ( Exception e )
        {
            return new ServiceResponse<bool>( false, false, e.Message );
        }

    }
    public async Task<ServiceResponse<bool>> AuthorizeUser()
    {
        ServiceResponse<UserSessionData?> localSession = await GetUserSession();

        if ( !localSession.Success )
            return new ServiceResponse<bool>( false, false, localSession.Message! );

        try
        {
            HttpResponseMessage httpResponse = await _http.PostAsJsonAsync( API_PATH_AUTHORIZE, new SessionApiRequest( localSession.Data!.Username, localSession.Data.Token ) );
            var serviceResponse = await httpResponse.Content.ReadFromJsonAsync<ServiceResponse<bool>>();

            return serviceResponse ?? new ServiceResponse<bool>( false, false, "Authorization response is null!" );
        }
        catch ( Exception e )
        {
            return new ServiceResponse<bool>( false, false, e.Message );
        }
    }
    public async Task<ServiceResponse<bool>> ChangePassword( UserChangePasswordRequest request )
    {
        try
        {
            ServiceResponse<UserSessionData?> localSession = await GetUserSession();

            if ( !localSession.Success )
                return new ServiceResponse<bool>( false, false, localSession.Message! );

            request.ApiRequest = new SessionApiRequest( localSession.Data!.Username, localSession.Data.Token );
            
            HttpResponseMessage httpResponse = await _http.PostAsJsonAsync( API_PATH_CHANGE_PASSWORD, request );
            var serviceResponse = await httpResponse.Content.ReadFromJsonAsync<ServiceResponse<bool>>();

            if ( serviceResponse == null )
                return new ServiceResponse<bool>( false, false, "Service response is null!" );

            return !serviceResponse.Success
                ? new ServiceResponse<bool>( false, false, serviceResponse.Message ??= "Failed to change user password; message is null!" )
                : serviceResponse;
        }
        catch ( Exception e )
        {
            return new ServiceResponse<bool>( false, false, e.Message );
        }
    }

    async Task<ServiceResponse<UserSessionData?>> GetUserSession()
    {
        var sessionData = await _localStorage.GetItemAsync<UserSessionData>( SESSION_DATA_KEY );

        if ( sessionData == null )
            return new ServiceResponse<UserSessionData?>( null, false, "Session object is null!" );

        if ( !sessionData.Validate( out string message ) )
            return new ServiceResponse<UserSessionData?>( null, false, message );

        return new ServiceResponse<UserSessionData?>( sessionData, true, "Successfully got session data." );
    }
}