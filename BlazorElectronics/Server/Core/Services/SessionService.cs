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

    readonly ISessionRepository _sessionRepository;

    public SessionService( ILogger<ApiService> logger, ISessionRepository sessionRepository ) 
        : base( logger )
    {
        _sessionRepository = sessionRepository;
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
            SessionValidationModel? session = await _sessionRepository.GetSessionValidation( sessionId );
            return await AuthorizeSession( session, sessionId, sessionToken, mustBeAdmin );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<int>( ServiceErrorType.ServerError );
        }
    }

    async Task<ServiceReply<int>> AuthorizeSession( SessionValidationModel? session, int sessionId, string sessionToken, bool mustBeAdmin = false )
    {
        if ( session is null )
            return new ServiceReply<int>( ServiceErrorType.NotFound );

        if ( !session.IsValid( MAX_SESSION_HOURS ) )
        {
            await DeleteSession( sessionId );
            return new ServiceReply<int>( ServiceErrorType.Unauthorized, SESSION_EXPIRED_MESSAGE );
        }

        if ( !VerifySessionToken( sessionToken, session.TokenHash, session.TokenSalt ) )
            return new ServiceReply<int>( ServiceErrorType.Unauthorized, SESSION_EXPIRED_MESSAGE );

        if ( !session.IsActive )
            return new ServiceReply<int>( ServiceErrorType.Unauthorized, "Account is not active!" );

        if ( !mustBeAdmin )
            return new ServiceReply<int>( session.UserId );

        return session.IsAdmin
            ? new ServiceReply<int>( session.UserId )
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