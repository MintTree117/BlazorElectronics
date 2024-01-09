using System.Security.Cryptography;
using System.Text;
using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Server.Core.Models.Sessions;
using BlazorElectronics.Server.Core.Models.Users;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.Sessions;

namespace BlazorElectronics.Server.Core.Services;

public sealed class SessionService : _ApiService, ISessionService
{
    const int MAX_SESSION_HOURS = 48;
    const string SESSION_EXPIRED_MESSAGE = "Session has expired. Please login again.";

    readonly ISessionRepository _sessionRepository;

    public SessionService( ILogger<SessionService> logger, ISessionRepository sessionRepository ) 
        : base( logger )
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<ServiceReply<List<SessionInfoDto>?>> GetUserSessions( int userId )
    {
        try
        {
            IEnumerable<SessionModel>? models = await _sessionRepository.GetSessions( userId );
            List<SessionInfoDto>? dto = MapSessionInfo( models );

            return dto is not null
                ? new ServiceReply<List<SessionInfoDto>?>( dto )
                : new ServiceReply<List<SessionInfoDto>?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<List<SessionInfoDto>?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<SessionDto?>> CreateSession( UserLoginDto dto, UserDeviceInfoDto? deviceInfo )
    {
        try
        {
            CreateSessionToken( out string token, out byte[] hash, out byte[] salt );
            SessionModel? insertedSession = await _sessionRepository.InsertSession( dto.UserId, hash, salt, deviceInfo );

            if ( insertedSession is null )
                return new ServiceReply<SessionDto?>( ServiceErrorType.Conflict, "Failed to create session!" );

            SessionDto session = new()
            {
                Username = dto.Username,
                SessionId = insertedSession.SessionId,
                SessionToken = token,
                IsAdmin = dto.IsAdmin
            };

            return new ServiceReply<SessionDto?>( session );
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
    public async Task<ServiceReply<bool>> DeleteAllSessions( int userId )
    {
        try
        {
            bool success = await _sessionRepository.DeleteAllSessions( userId );

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
    public async Task<ServiceReply<int>> AuthorizeSessionWithUserId( int sessionId, string sessionToken, UserDeviceInfoDto? deviceInfo, bool mustBeAdmin = false )
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
    static List<SessionInfoDto>? MapSessionInfo( IEnumerable<SessionModel>? models )
    {
        return models?
            .Select( m => new SessionInfoDto 
                { 
                    SessionId = m.SessionId, 
                    DateCreated = m.DateCreated,
                    LastActive = m.LastActivityDate, 
                    IpAddress = m.IpAddress ?? string.Empty 
                } )
            .ToList();
    }
}