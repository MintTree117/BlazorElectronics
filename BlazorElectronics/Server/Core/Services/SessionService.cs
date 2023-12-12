using System.Security.Cryptography;
using System.Text;
using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Server.Core.Models.Sessions;
using BlazorElectronics.Server.Core.Models.Users;
using BlazorElectronics.Server.Data;
using BlazorElectronics.Server.Services;
using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Server.Core.Services;

public class SessionService : ApiService, ISessionService
{
    const int MAX_SESSION_HOURS = 48;
    const string SESSION_EXPIRED_MESSAGE = "Session has expired. Please login again.";
    const string INVALID_SESSION_TOKEN_MESSAGE = "Invalid session token. Try loging in again.";
    
    readonly ISessionRepository _sessionRepository;
    
    public SessionService( ILogger<ApiService> logger, ISessionRepository sessionRepository ) : base( logger )
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<ServiceReply<SessionDto?>> CreateSession( int userId, UserDeviceInfoDto? deviceInfo )
    {
        CreateSessionToken( out string token, out byte[] hash, out byte[] salt );

        UserSession? insertedSession;
        
        try
        {
            insertedSession = await _sessionRepository.InsertSession( userId, hash, salt, deviceInfo );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<SessionDto?>( ServiceErrorType.ServerError );
        }
        
        return insertedSession is not null 
            ? new ServiceReply<SessionDto?>( new SessionDto( insertedSession.SessionId, token ) ) 
            : new ServiceReply<SessionDto?>( ServiceErrorType.NotFound );
    }
    public async Task<ServiceReply<bool>> DeleteSession( int sessionId )
    {
        bool success;

        try
        {
            success = await _sessionRepository.DeleteSession( sessionId );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }

        return success
            ? new ServiceReply<bool>( true )
            : new ServiceReply<bool>( ServiceErrorType.NotFound );

    }
    public async Task<ServiceReply<bool>> AuthorizeSession( int sessionId, string sessionToken, UserDeviceInfoDto? deviceInfo )
    {
        UserSession? session;

        try
        {
            session = await _sessionRepository.GetSession( sessionId );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }

        if ( session is null )
            return new ServiceReply<bool>( ServiceErrorType.NotFound );

        if ( !session.IsValid( MAX_SESSION_HOURS ) )
        {
            await DeleteSession( session.SessionId );
            return new ServiceReply<bool>( ServiceErrorType.ValidationError, SESSION_EXPIRED_MESSAGE );
        }

        return VerifySessionToken( sessionToken, session.TokenHash, session.TokenSalt )
            ? new ServiceReply<bool>( true )
            : new ServiceReply<bool>( ServiceErrorType.ValidationError, INVALID_SESSION_TOKEN_MESSAGE );
    }
    public async Task<ServiceReply<int>> AuthorizeSessionId( int sessionId, string sessionToken, UserDeviceInfoDto? deviceInfo )
    {
        UserSession? session;

        try
        {
            session = await _sessionRepository.GetSession( sessionId );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<int>( ServiceErrorType.ServerError );
        }

        if ( session is null )
            return new ServiceReply<int>( ServiceErrorType.NotFound );

        if ( !session.IsValid( MAX_SESSION_HOURS ) )
        {
            await DeleteSession( session.SessionId );
            return new ServiceReply<int>( ServiceErrorType.ValidationError, SESSION_EXPIRED_MESSAGE );   
        }

        return VerifySessionToken( sessionToken, session.TokenHash, session.TokenSalt )
            ? new ServiceReply<int>( session.UserId )
            : new ServiceReply<int>( ServiceErrorType.ValidationError, INVALID_SESSION_TOKEN_MESSAGE );
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