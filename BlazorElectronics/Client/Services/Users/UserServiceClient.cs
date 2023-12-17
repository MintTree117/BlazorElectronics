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
    
    SessionReplyDto? _userSession;
    
    public async Task<ServiceReply<bool>> Register( RegisterRequestDto requestDto )
    {
        ServiceReply<SessionReplyDto?> registerReply = await TryGetSessionResponse( API_ROUTE_REGISTER, requestDto );
        return new ServiceReply<bool>( false );
    }
    public async Task<ServiceReply<SessionReplyDto?>> Login( LoginRequestDto requestDto )
    {
        ServiceReply<SessionReplyDto?> loginReply = await TryGetSessionResponse( API_ROUTE_LOGIN, requestDto );

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
    public async Task<ServiceReply<bool>> ChangePassword( PasswordRequestDto requestDto )
    {
        return await TryUserRequest<PasswordRequestDto, bool>( API_ROUTE_CHANGE_PASSWORD, requestDto );
    }
    public async Task<SessionMeta?> GetSessionMeta()
    {
        ServiceReply<SessionReplyDto?> reply = await TryGetLocalUserSession();

        if ( !reply.Success || reply.Data is null )
            return null;

        return new SessionMeta( reply.Data.IsAdmin ? SessionType.Admin : SessionType.User, reply.Data.Username );
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

    protected async Task<ServiceReply<T?>> TryUserRequest<T>( string apiRoute )
    {
        ServiceReply<SessionReplyDto?> sessionReply = await TryGetLocalUserSession();
        
        if ( !sessionReply.Success || sessionReply.Data is null )
        {
            Logger.LogError( sessionReply.ErrorType + sessionReply.Message );
            return new ServiceReply<T?>( sessionReply.ErrorType );
        }

        var apiRequest = new UserRequestDto( sessionReply.Data );
        return await TryPostRequest<T?>( apiRoute, apiRequest );
    }
    protected async Task<ServiceReply<RESPONSE?>> TryUserRequest<REQUEST,RESPONSE>( string apiRoute, REQUEST payload ) where REQUEST : class
    {
        ServiceReply<SessionReplyDto?> sessionReply = await TryGetLocalUserSession();

        if ( !sessionReply.Success || sessionReply.Data is null )
        {
            Logger.LogError( sessionReply.Message );
            return new ServiceReply<RESPONSE?>( sessionReply.ErrorType );
        }

        var apiRequest = new UserDataRequestDto<REQUEST>( sessionReply.Data, payload );
        return await TryPostRequest<RESPONSE?>( apiRoute, apiRequest );
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

    static SessionMeta GetSessionMeta( SessionReplyDto sessionReply )
    {
        return new SessionMeta( sessionReply.IsAdmin ? SessionType.Admin : SessionType.User, sessionReply.Username );
    }
}