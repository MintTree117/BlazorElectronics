using System.Runtime.CompilerServices;
using Blazored.LocalStorage;
using BlazorElectronics.Client.Models;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.Users;

namespace BlazorElectronics.Client.Services.Users;

public class UserServiceClient : ClientService, IUserServiceClient
{
    public UserServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage )
    {
        
    }

    public event IUserServiceClient.AsyncEventHandler? SessionChanged;

    const string API_ROUTE = "api/UserAccount";
    const string API_ROUTE_REGISTER = API_ROUTE + "/register";
    const string API_ROUTE_LOGIN = API_ROUTE + "/login";
    const string API_ROUTE_LOGOUT = API_ROUTE + "/logout";
    const string API_ROUTE_AUTHORIZE = API_ROUTE + "/authorize";
    const string API_ROUTE_CHANGE_PASSWORD = API_ROUTE + "/change-password";
    const string API_ROUTE_ACTIVATE_ACCOUNT = API_ROUTE + "/activate";
    const string SESSION_DATA_KEY = "UserSession";
    
    SessionReplyDto? _userSession;
    
    public async Task<ServiceReply<bool>> Register( RegisterRequestDto requestDto )
    {
        ServiceReply<bool> registerReply = await TryPostRequest<bool>( API_ROUTE_REGISTER, requestDto );
        return registerReply;
    }
    public async Task<ServiceReply<bool>> ActivateAccount( string token )
    {
        return await TryPostRequest<bool>( API_ROUTE_ACTIVATE_ACCOUNT, token );
    }
    public async Task<ServiceReply<SessionReplyDto?>> Login( LoginRequestDto requestDto )
    {
        ServiceReply<SessionReplyDto?> loginReply = await TryGetSessionResponse( API_ROUTE_LOGIN, requestDto );

        if ( loginReply is { Success: true, Data: not null } )
            await InvokeOnChange( GetSessionMeta( loginReply.Data ) );

        return loginReply;
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
    public async Task<ServiceReply<bool>> Logout()
    {
        ServiceReply<bool> serverReply = await TryUserPostRequest<bool>( API_ROUTE_LOGOUT );
        await Storage.RemoveItemAsync( SESSION_DATA_KEY );
        await InvokeOnChange( null );
        return serverReply;
    }
    public async Task<ServiceReply<bool>> ChangePassword( PasswordRequestDto requestDto )
    {
        return await TryUserPostRequest<bool>( API_ROUTE_CHANGE_PASSWORD, requestDto );
    }
    public async Task<SessionMeta?> GetSessionMeta()
    {
        ServiceReply<SessionReplyDto?> reply = await TryGetLocalUserSession();

        if ( !reply.Success || reply.Data is null )
            return null;

        return new SessionMeta( reply.Data.IsAdmin ? SessionType.Admin : SessionType.User, reply.Data.Username );
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
    
    async Task InvokeOnChange( SessionMeta? session )
    {
        IUserServiceClient.AsyncEventHandler? handler = SessionChanged;
        if ( handler != null )
        {
            Delegate[] invocationList = handler.GetInvocationList();
            var handlerTasks = new List<Task>();
            foreach ( Delegate @delegate in invocationList )
            {
                var singleHandler = ( IUserServiceClient.AsyncEventHandler ) @delegate;
                Task task = singleHandler.Invoke( this, session );
                handlerTasks.Add( task );
            }
            await Task.WhenAll( handlerTasks );
        }
    }
    async Task<ServiceReply<SessionReplyDto?>> TryGetLocalUserSession()
    {
        if ( _userSession is not null )
            return new ServiceReply<SessionReplyDto?>( _userSession );

        try
        {
            _userSession = await Storage.GetItemAsync<SessionReplyDto?>( SESSION_DATA_KEY );
        }
        catch ( Exception e )
        {
            Logger.LogError( e.Message + e.InnerException?.Message );
            return new ServiceReply<SessionReplyDto?>( ServiceErrorType.IoError );
        }

        return _userSession is not null 
            ? new ServiceReply<SessionReplyDto?>( _userSession ) 
            : new ServiceReply<SessionReplyDto?>( ServiceErrorType.NotFound );
    }
    async Task<ServiceReply<SessionReplyDto?>> TryGetSessionResponse<T>( string apiPath, T requestObject )
    {
        ServiceReply<SessionReplyDto?> sessionReply = await TryPostRequest<SessionReplyDto?>( apiPath, requestObject );

        if ( !sessionReply.Success || sessionReply.Data is null )
            return sessionReply;
        
        _userSession = sessionReply.Data;

        await TryStoreSessionResponse();
        
        return new ServiceReply<SessionReplyDto?>( _userSession );
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
        ServiceReply<SessionReplyDto?> sessionReply = await TryGetLocalUserSession();

        if ( !sessionReply.Success || sessionReply.Data is null )
            return false;

        int sessionId = sessionReply.Data.SessionId;
        string sessionToken = sessionReply.Data.SessionToken;

        Http.DefaultRequestHeaders.Add( "SessionId", sessionId.ToString() );
        Http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue( "Bearer", sessionToken );

        return true;
    }

    static SessionMeta GetSessionMeta( SessionReplyDto sessionReply )
    {
        return new SessionMeta( sessionReply.IsAdmin ? SessionType.Admin : SessionType.User, sessionReply.Username );
    }
}