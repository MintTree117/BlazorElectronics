using System.Security.Cryptography;
using BlazorElectronics.Server.Dtos.Users;
using BlazorElectronics.Server.Models.Users;
using BlazorElectronics.Server.Repositories.Sessions;

namespace BlazorElectronics.Server.Services.Sessions;

public class SessionService : ApiService, ISessionService
{
    const int MAX_SESSION_HOURS = 48;
    const string SESSION_EXPIRED_MESSAGE = "Session has expired. Please login again.";
    const string INVALID_SESSION_TOKEN_MESSAGE = "Invalid session token. Try loging in again.";
    
    readonly ISessionRepository _sessionRepository;
    
    public SessionService( ILogger logger, ISessionRepository sessionRepository ) : base( logger )
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<ApiReply<string?>> CreateSession( int userId, UserDeviceInfoDto? deviceInfo )
    {
        CreateSessionToken( out string token, out byte[] hash, out byte[] salt );

        try
        {
            UserSession? insertedSession = await _sessionRepository.AddSession( userId, hash, salt, deviceInfo );

            if ( insertedSession is null )
                return new ApiReply<string?>( NO_DATA_FOUND_MESSAGE );
        }
        catch ( ServiceException e )
        {
            _logger.LogError( e.Message, e );
            return new ApiReply<string?>( INTERNAL_SERVER_ERROR_MESSAGE );
        }

        return new ApiReply<string?>( token );
    }
    public async Task<ApiReply<bool>> ValidateSession( int userId, string sessionToken, UserDeviceInfoDto? deviceInfo )
    {
        UserSession? session;

        try
        {
            session = await _sessionRepository.GetSession( userId, deviceInfo );

            if ( session is null )
                return new ApiReply<bool>( NO_DATA_FOUND_MESSAGE );

            if ( !session.IsValid( MAX_SESSION_HOURS ) )
                return new ApiReply<bool>( SESSION_EXPIRED_MESSAGE );
        }
        catch ( ServiceException e )
        {
            _logger.LogError( e.Message, e );
            return new ApiReply<bool>( INTERNAL_SERVER_ERROR_MESSAGE );
        }

        return VerifySessionToken( sessionToken, session.Hash, session.Salt )
            ? new ApiReply<bool>( true )
            : new ApiReply<bool>( INVALID_SESSION_TOKEN_MESSAGE );
    }

    static void CreateSessionToken( out string token, out byte[] hash, out byte[] salt )
    {
        var hmac = new HMACSHA512();

        byte[] tokenBytes = new byte[ 32 ];
        using ( var rng = RandomNumberGenerator.Create() )
            rng.GetBytes( tokenBytes );
        token = Convert.ToBase64String( tokenBytes );

        salt = hmac.Key;
        hash = hmac.ComputeHash( tokenBytes );

        hmac.Dispose();
    }
    static bool VerifySessionToken( string token, byte[] hash, byte[] salt )
    {
        var hmac = new HMACSHA512( salt );
        byte[] computedHash = hmac.ComputeHash( System.Text.Encoding.UTF8.GetBytes( token ) );
        return computedHash.SequenceEqual( hash );
    }
}