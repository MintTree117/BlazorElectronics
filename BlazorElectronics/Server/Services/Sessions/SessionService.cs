using System.Security.Cryptography;
using System.Text;
using BlazorElectronics.Server.Dtos.Sessions;
using BlazorElectronics.Server.Dtos.Users;
using BlazorElectronics.Server.Models.Users;
using BlazorElectronics.Server.Repositories.Sessions;

namespace BlazorElectronics.Server.Services.Sessions;

public class SessionService : ApiService<SessionService>, ISessionService
{
    const int MAX_SESSION_HOURS = 48;
    const string SESSION_EXPIRED_MESSAGE = "Session has expired. Please login again.";
    const string INVALID_SESSION_TOKEN_MESSAGE = "Invalid session token. Try loging in again.";
    
    readonly ISessionRepository _sessionRepository;
    
    public SessionService( ILogger<SessionService> logger, ISessionRepository sessionRepository ) : base( logger )
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<ApiReply<SessionDto?>> CreateSession( int userId, UserDeviceInfoDto? deviceInfo )
    {
        CreateSessionToken( out string token, out byte[] hash, out byte[] salt );

        UserSession? insertedSession;
        
        try
        {
            insertedSession = await _sessionRepository.AddSession( userId, hash, salt, deviceInfo );
        }
        catch ( ServiceException e )
        {
            _logger.LogError( e.Message, e );
            return new ApiReply<SessionDto?>( INTERNAL_SERVER_ERROR_MESSAGE );
        }

        return insertedSession is not null ? 
            new ApiReply<SessionDto?>( new SessionDto( insertedSession.SessionId, token ) ) 
            : new ApiReply<SessionDto?>( NO_DATA_FOUND_MESSAGE );
    }
    public async Task<ApiReply<int>> AuthorizeSession( int sessionId, string sessionToken, UserDeviceInfoDto? deviceInfo )
    {
        UserSession? session;

        try
        {
            session = await _sessionRepository.GetSession( sessionId );
        }
        catch ( ServiceException e )
        {
            _logger.LogError( e.Message, e );
            return new ApiReply<int>( INTERNAL_SERVER_ERROR_MESSAGE );
        }

        if ( session is null )
            return new ApiReply<int>( NO_DATA_FOUND_MESSAGE );

        if ( !session.IsValid( MAX_SESSION_HOURS ) )
            return new ApiReply<int>( SESSION_EXPIRED_MESSAGE );

        return VerifySessionToken( sessionToken, session.Hash, session.Salt )
            ? new ApiReply<int>( session.UserId )
            : new ApiReply<int>( INVALID_SESSION_TOKEN_MESSAGE );
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
        
        // Convert the Base64 string back to a byte array before hashing
        byte[] tokenBytes = Convert.FromBase64String( token );
        byte[] computedHash = hmac.ComputeHash( tokenBytes );
        
        hmac.Dispose();
        
        return computedHash.SequenceEqual( hash );
    }
}