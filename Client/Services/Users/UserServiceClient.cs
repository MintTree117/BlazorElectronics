using Blazored.LocalStorage;
using BlazorElectronics.Client.Models;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.Sessions;
using BlazorElectronics.Shared.Users;

namespace BlazorElectronics.Client.Services.Users;

public class UserServiceClient : ClientService, IUserServiceClient
{
    const string SESSION_DATA_KEY = "UserSession";
    const string SESSION_ID_HEADER = "SessionId";
    const string SESSION_TOKEN_HEADER = "Bearer";
    
    public UserServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }

    public event IUserServiceClient.AsyncEventHandler? SessionChanged;

    const string API_ROUTE = "api/UserAccount";
    
    const string API_ROUTE_AUTHORIZE = API_ROUTE + "/authorize";
    const string API_ROUTE_LOGIN = API_ROUTE + "/login";
    const string API_ROUTE_LOGOUT = API_ROUTE + "/logout";
    
    const string API_ROUTE_REGISTER = API_ROUTE + "/register";
    const string API_ROUTE_ACTIVATE_ACCOUNT = API_ROUTE + "/activate";
    const string API_ROUTE_RESEND_VERIFICATION = API_ROUTE + "/resend-verification";
    
    const string API_ROUTE_GET_DETAILS = API_ROUTE + "/get-account-details";
    const string API_ROUTE_GET_SESSIONS = API_ROUTE + "/get-sessions";

    const string API_ROUTE_UPDATE_DETAILS = API_ROUTE + "/update-account-details";
    const string API_ROUTE_CHANGE_PASSWORD = API_ROUTE + "/update-password";
    const string API_ROUTE_DELETE_SESSION = API_ROUTE + "/delete-session";
    const string API_ROUTE_DELETE_ALL_SESSION = API_ROUTE + "/delete-all-sessions";
    
    SessionDto? _userSession;

    public async Task<SessionMeta?> GetSessionMeta()
    {
        ServiceReply<SessionDto?> reply = await TryGetLocalUserSession();

        if ( !reply.Success || reply.Payload is null )
            return null;

        return new SessionMeta( reply.Payload.IsAdmin ? SessionType.Admin : SessionType.User, reply.Payload.Username );
    }
    
    public async Task<ServiceReply<bool>> AuthorizeUser()
    {
        ServiceReply<bool> authorizeReply = await TryUserGetRequest<bool>( API_ROUTE_AUTHORIZE );

        if ( !authorizeReply.Success )
        {
            ServiceReply<bool> logoutReply = await Logout();

            if ( !logoutReply.Success )
                Logger.LogError( $"Failed to log user out! {logoutReply.ErrorType} : {logoutReply.Message}" );
        }

        return authorizeReply.Success
            ? new ServiceReply<bool>( true )
            : new ServiceReply<bool>( authorizeReply.ErrorType, authorizeReply.Message );
    }
    public async Task<ServiceReply<SessionDto?>> Login( LoginRequestDto loginRequest )
    {
        ServiceReply<SessionDto?> loginReply = await TryPostRequest<SessionDto?>( API_ROUTE_LOGIN, loginRequest );

        if ( loginReply is { Success: true, Payload: not null } )
            await InvokeOnChange( GetSessionMeta( loginReply.Payload ) );
        
        _userSession = loginReply.Payload;
        await TryStoreSessionResponse();

        return loginReply;
    }
    public async Task<ServiceReply<bool>> Logout()
    {
        await TryGetLocalUserSession();

        ServiceReply<bool> serverReply = await TryUserGetRequest<bool>( API_ROUTE_LOGOUT, GetSessionIdParam( _userSession!.SessionId ) );
        await Storage.RemoveItemAsync( SESSION_DATA_KEY );
        await InvokeOnChange( null );
        return serverReply;
    }
    
    public async Task<ServiceReply<bool>> Register( RegisterRequestDto requestDto )
    {
        ServiceReply<bool> registerReply = await TryPutRequest<bool>( API_ROUTE_REGISTER, requestDto );
        return registerReply;
    }
    public async Task<ServiceReply<bool>> ActivateAccount( string token )
    {
        return await TryPutRequest<bool>( API_ROUTE_ACTIVATE_ACCOUNT, token );
    }
    public async Task<ServiceReply<bool>> ResendVerification()
    {
        return await TryUserGetRequest<bool>( API_ROUTE_RESEND_VERIFICATION );
    }

    public async Task<ServiceReply<AccountDetailsDto?>> GetAccountDetails()
    {
        return await TryUserGetRequest<AccountDetailsDto?>( API_ROUTE_GET_DETAILS );
    }
    public async Task<ServiceReply<List<SessionInfoDto>?>> GetUserSessions()
    {
        return await TryUserGetRequest<List<SessionInfoDto>?>( API_ROUTE_GET_SESSIONS );
    }

    public async Task<ServiceReply<AccountDetailsDto?>> UpdateAccountDetails( AccountDetailsDto dto )
    {
        ServiceReply<AccountDetailsDto?> serverReply = await TryUserPostRequest<AccountDetailsDto?>( API_ROUTE_UPDATE_DETAILS, dto );

        if ( !serverReply.Success || serverReply.Payload is null )
            return serverReply;

        ServiceReply<SessionDto?> sessionReply = await TryGetLocalUserSession();

        if ( !sessionReply.Success || sessionReply.Payload is null || _userSession is null )
            return serverReply;

        _userSession.Username = serverReply.Payload.Username;

        await TryStoreSessionResponse();

        return serverReply;
    }
    public async Task<ServiceReply<bool>> UpdatePassword( PasswordRequestDto requestDto )
    {
        return await TryUserPostRequest<bool>( API_ROUTE_CHANGE_PASSWORD, requestDto );
    }
    public async Task<ServiceReply<bool>> DeleteSession( int sessionId )
    {
        return await TryUserDeleteRequest<bool>( API_ROUTE_DELETE_SESSION, GetSessionIdParam( sessionId ) );
    }
    public async Task<ServiceReply<bool>> DeleteAllSessions()
    {
        ServiceReply<bool> reply = await TryUserDeleteRequest<bool>( API_ROUTE_DELETE_ALL_SESSION );

        if ( !reply.Success ) 
            return reply;
        
        await Storage.RemoveItemAsync( SESSION_DATA_KEY );
        await InvokeOnChange( null );

        return reply;
    }

    protected async Task<ServiceReply<T?>> TryUserGetRequest<T>( string apiPath, Dictionary<string, object>? parameters = null )
    {
        if ( !await SetUserHttpHeader() )
            return new ServiceReply<T?>( ServiceErrorType.Unauthorized, "No session found!" );
        
        return await TryGetRequest<T?>( apiPath, parameters );
    }
    protected async Task<ServiceReply<T?>> TryUserPostRequest<T>( string apiPath, object? body = null )
    {
        if ( !await SetUserHttpHeader() )
            return new ServiceReply<T?>( ServiceErrorType.Unauthorized, "No session found!" );
        
        return await TryPostRequest<T?>( apiPath, body );
    }
    protected async Task<ServiceReply<T?>> TryUserPutRequest<T>( string apiPath, object? body = null )
    {
        if ( !await SetUserHttpHeader() )
            return new ServiceReply<T?>( ServiceErrorType.Unauthorized, "No session found!" );
        
        return await TryPutRequest<T?>( apiPath, body );
    }
    protected async Task<ServiceReply<T?>> TryUserDeleteRequest<T>( string apiPath, Dictionary<string, object>? parameters = null )
    {
        if ( !await SetUserHttpHeader() )
            return new ServiceReply<T?>( ServiceErrorType.Unauthorized, "No session found!" );
        
        return await TryDeleteRequest<T?>( apiPath, parameters );
    }

    async Task<ServiceReply<SessionDto?>> TryGetLocalUserSession()
    {
        if ( _userSession is not null )
            return new ServiceReply<SessionDto?>( _userSession );

        try
        {
            _userSession = await Storage.GetItemAsync<SessionDto?>( SESSION_DATA_KEY );
        }
        catch ( Exception e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<SessionDto?>( ServiceErrorType.IoError );
        }

        return _userSession is not null
            ? new ServiceReply<SessionDto?>( _userSession )
            : new ServiceReply<SessionDto?>( ServiceErrorType.NotFound );
    }
    async Task InvokeOnChange( SessionMeta? session )
    {
        IUserServiceClient.AsyncEventHandler? handler = SessionChanged;

        if ( handler is null )
            return;

        Delegate[] invocationList = handler.GetInvocationList();
        
        List<Task> handlerTasks = ( 
            from IUserServiceClient.AsyncEventHandler? singleHandler 
                in invocationList 
            select singleHandler
                .Invoke( this, session ) )
            .ToList();

        await Task.WhenAll( handlerTasks );
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
    async Task<bool> SetUserHttpHeader()
    {
        ServiceReply<SessionDto?> sessionReply = await TryGetLocalUserSession();

        if ( !sessionReply.Success || sessionReply.Payload is null )
            return false;

        int sessionId = sessionReply.Payload.SessionId;
        string sessionToken = sessionReply.Payload.SessionToken;

        if ( Http.DefaultRequestHeaders.Contains( SESSION_ID_HEADER ) )
            Http.DefaultRequestHeaders.Remove( SESSION_ID_HEADER );

        Http.DefaultRequestHeaders.Add( SESSION_ID_HEADER, sessionId.ToString() );
        Http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue( SESSION_TOKEN_HEADER, sessionToken );

        return true;
    }

    static Dictionary<string, object> GetSessionIdParam( int sessionId )
    {
        return new Dictionary<string, object> { { "sessionId", sessionId } };
    }
    static SessionMeta GetSessionMeta( SessionDto session )
    {
        return new SessionMeta( session.IsAdmin ? SessionType.Admin : SessionType.User, session.Username );
    }
}