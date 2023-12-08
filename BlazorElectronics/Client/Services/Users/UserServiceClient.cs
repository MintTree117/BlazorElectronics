using Blazored.LocalStorage;
using BlazorElectronics.Client.Models;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.Users;

namespace BlazorElectronics.Client.Services.Users;

public class UserServiceClient : ClientService, IUserServiceClient
{
    public UserServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }

    public event Action<SessionMeta?>? SessionChanged;

    const string API_ROUTE = "api/UserAccount";
    const string API_ROUTE_REGISTER = API_ROUTE + "/register";
    const string API_ROUTE_LOGIN = API_ROUTE + "/login";
    const string API_ROUTE_LOGOUT = API_ROUTE + "/logout";
    const string API_ROUTE_AUTHORIZE = API_ROUTE + "/authorize";
    const string API_ROUTE_CHANGE_PASSWORD = API_ROUTE + "/change-password";
    const string SESSION_DATA_KEY = "UserSession";
    
    UserSessionResponse? _userSession;
    
    public async Task<ServiceReply<bool>> Register( UserRegisterRequest request )
    {
        ServiceReply<UserSessionResponse?> registerReply = await TryGetSessionResponse( API_ROUTE_REGISTER, request );
        return new ServiceReply<bool>( false );
    }
    public async Task<ServiceReply<UserSessionResponse?>> Login( UserLoginRequest request )
    {
        ServiceReply<UserSessionResponse?> loginReply = await TryGetSessionResponse( API_ROUTE_LOGIN, request );

        if ( loginReply is { Success: true, Data: not null } )
            SessionChanged?.Invoke( GetSessionMeta( loginReply.Data ) );

        return loginReply;
    }
    public async Task<ServiceReply<bool>> AuthorizeUser()
    {
        ServiceReply<bool> serviceReply = await TryUserRequest<bool>( API_ROUTE_AUTHORIZE );

        return serviceReply.Success
            ? new ServiceReply<bool>( true )
            : new ServiceReply<bool>( serviceReply.ErrorType, serviceReply.Message );
    }
    public async Task<ServiceReply<bool>> Logout()
    {
        ServiceReply<bool> serverReply = await TryUserRequest<bool>( API_ROUTE_LOGOUT );
        await Storage.RemoveItemAsync( SESSION_DATA_KEY );
        SessionChanged?.Invoke( null );
        return serverReply;
    }
    public async Task<ServiceReply<bool>> ChangePassword( PasswordChangeRequest request )
    {
        return await TryUserRequest<PasswordChangeRequest, bool>( API_ROUTE_CHANGE_PASSWORD, request );
    }
    public async Task<SessionMeta?> GetSessionMeta()
    {
        ServiceReply<UserSessionResponse?> reply = await TryGetLocalUserSession();

        if ( !reply.Success || reply.Data is null )
            return null;

        return new SessionMeta( reply.Data.IsAdmin ? SessionType.Admin : SessionType.User, reply.Data.Username );
    }
    async Task<ServiceReply<UserSessionResponse?>> TryGetLocalUserSession()
    {
        if ( _userSession is not null )
            return new ServiceReply<UserSessionResponse?>( _userSession );

        try
        {
            _userSession = await Storage.GetItemAsync<UserSessionResponse?>( SESSION_DATA_KEY );
        }
        catch ( Exception e )
        {
            Logger.LogError( e.Message + e.InnerException?.Message );
            return new ServiceReply<UserSessionResponse?>( ServiceErrorType.IoError );
        }

        return _userSession is not null 
            ? new ServiceReply<UserSessionResponse?>( _userSession ) 
            : new ServiceReply<UserSessionResponse?>( ServiceErrorType.NotFound );
    }

    protected async Task<ServiceReply<T?>> TryUserRequest<T>( string apiRoute )
    {
        ServiceReply<UserSessionResponse?> sessionReply = await TryGetLocalUserSession();
        
        if ( !sessionReply.Success || sessionReply.Data is null )
        {
            Logger.LogError( sessionReply.ErrorType + sessionReply.Message );
            return new ServiceReply<T?>( sessionReply.ErrorType );
        }

        var apiRequest = new UserRequest( sessionReply.Data );
        return await TryPostRequest<T?>( apiRoute, apiRequest );
    }
    protected async Task<ServiceReply<RESPONSE?>> TryUserRequest<REQUEST,RESPONSE>( string apiRoute, REQUEST payload ) where REQUEST : class
    {
        ServiceReply<UserSessionResponse?> sessionReply = await TryGetLocalUserSession();

        if ( !sessionReply.Success || sessionReply.Data is null )
        {
            Logger.LogError( sessionReply.Message );
            return new ServiceReply<RESPONSE?>( sessionReply.ErrorType );
        }

        var apiRequest = new UserDataRequest<REQUEST>( sessionReply.Data, payload );
        return await TryPostRequest<RESPONSE?>( apiRoute, apiRequest );
    }
    async Task<ServiceReply<UserSessionResponse?>> TryGetSessionResponse<T>( string apiPath, T requestObject )
    {
        ServiceReply<UserSessionResponse?> sessionReply = await TryPostRequest<UserSessionResponse?>( apiPath, requestObject );

        if ( !sessionReply.Success || sessionReply.Data is null )
            return sessionReply;
        
        _userSession = sessionReply.Data;

        await TryStoreSessionResponse();
        
        return new ServiceReply<UserSessionResponse?>( _userSession );
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

    static SessionMeta GetSessionMeta( UserSessionResponse session )
    {
        return new SessionMeta( session.IsAdmin ? SessionType.Admin : SessionType.User, session.Username );
    }
}