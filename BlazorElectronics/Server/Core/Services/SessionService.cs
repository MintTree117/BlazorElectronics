using System.Security.Cryptography;
using System.Text;
using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Server.Core.Models.Sessions;
using BlazorElectronics.Server.Core.Models.Users;
using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Server.Core.Services;

public sealed class SessionService : ApiService, ISessionService
{
    const int MAX_SESSION_HOURS = 48;
    const string SESSION_EXPIRED_MESSAGE = "Session has expired. Please login again.";
    const string INVALID_SESSION_TOKEN_MESSAGE = "Invalid session token. Try loging in again.";
    
    readonly ISessionRepository _sessionRepository;
    readonly IUserRepository _userRepository;
    
    public SessionService( ILogger<ApiService> logger, ISessionRepository sessionRepository, IUserRepository userRepository ) 
        : base( logger )
    {
        _sessionRepository = sessionRepository;
        _userRepository = userRepository;
    }

    public async Task<ServiceReply<SessionDto?>> CreateSession( int userId, UserDeviceInfoDto? deviceInfo )
    {
        try
        {
            CreateSessionToken( out string token, out byte[] hash, out byte[] salt );
            UserSession? insertedSession = await _sessionRepository.InsertSession( userId, hash, salt, deviceInfo );

            return insertedSession is not null
                ? new ServiceReply<SessionDto?>( new SessionDto( insertedSession.SessionId, token ) )
                : new ServiceReply<SessionDto?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<SessionDto?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<bool>> DeleteSession( int sessionId )
    {
        try
        {
            bool success = await _sessionRepository.DeleteSession( sessionId );

            return success
                ? new ServiceReply<bool>( true )
                : new ServiceReply<bool>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<int>> AuthorizeSessionAndUserId( int sessionId, string sessionToken, UserDeviceInfoDto? deviceInfo, bool mustBeAdmin = false )
    {
        try
        {
            UserSession? session = await _sessionRepository.GetSession( sessionId );
            ServiceReply<int> validateReply = await AuthorizeSession( session, sessionToken );

            if ( !validateReply.Success )
                return validateReply;

            UserValidationModel? user = await _userRepository.GetValidation( session!.UserId );

            return AuthorizeUser( session.UserId, user, mustBeAdmin );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<int>( ServiceErrorType.ServerError );
        }
    }

    async Task<ServiceReply<int>> AuthorizeSession( UserSession? session, string sessionToken )
    {
        if ( session is null )
            return new ServiceReply<int>( ServiceErrorType.NotFound );

        if ( !session.IsValid( MAX_SESSION_HOURS ) )
        {
            await DeleteSession( session.SessionId );
            return new ServiceReply<int>( ServiceErrorType.Unauthorized, SESSION_EXPIRED_MESSAGE );
        }

        return VerifySessionToken( sessionToken, session.TokenHash, session.TokenSalt )
            ? new ServiceReply<int>( session.SessionId )
            : new ServiceReply<int>( ServiceErrorType.Unauthorized, INVALID_SESSION_TOKEN_MESSAGE );
    }
    ServiceReply<int> AuthorizeUser( int userId, UserValidationModel? user, bool mustBeAdmin = false )
    {
        if ( user is null )
            return new ServiceReply<int>( ServiceErrorType.NotFound );

        if ( !user.IsActive )
            return new ServiceReply<int>( ServiceErrorType.Unauthorized, "Account is not active!" );

        if ( !mustBeAdmin )
            return new ServiceReply<int>( userId );

        return user.IsAdmin
            ? new ServiceReply<int>( userId )
            : new ServiceReply<int>( ServiceErrorType.Unauthorized, "Account is not an admin!" );
    }

    static void CreateSessionToken( out string token, out byte[] hash, out byte[] salt )
    {
        var hmac = new HMACSHA512();

        byte[] tokenBytes = new byte[ 32 ];
        using ( var rng = RandomNumberGenerator.Create() )
            rng.GetBytes( tokenBytes );
        
        token = Convert.ToBase64String( tokenBytes );
        salt = hmac.Key;
        hash = hmac.ComputeHash( Encoding.UTF8.GetBytes( token ) );
        
        hmac.Dispose();
    }
    static bool VerifySessionToken( string token, byte[] hash, byte[] salt )
    {
        var hmac = new HMACSHA512( salt );

        byte[] computedHash = hmac.ComputeHash( Encoding.UTF8.GetBytes( token ) );

        hmac.Dispose();
        
        return computedHash.SequenceEqual( hash );
    }
}