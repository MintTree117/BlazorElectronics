using System.Security.Cryptography;
using BlazorElectronics.Server.Models.Users;
using BlazorElectronics.Server.Repositories.Sessions;

namespace BlazorElectronics.Server.Services.Sessions;

public class SessionService : ISessionService
{
    readonly ISessionRepository _sessionRepository;
    
    public SessionService( ISessionRepository sessionRepository )
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<ServiceResponse<string?>> CreateNewSession( int userId, string ipAddress )
    {
        var session = new UserSession {
            UserId = userId,
            DateCreated = DateTime.Now,
            LastActivityDate = DateTime.Now,
            IsActive = true,
            IpAddress = ipAddress
        };

        CreateSessionToken( out string token, out byte[] hash, out byte[] salt );
        session.Hash = hash;
        session.Salt = salt;

        session = await _sessionRepository.CreateSession( session );

        return session == null 
            ? new ServiceResponse<string?>( null, false, $"Failed to create session for user {userId}!" ) 
            : new ServiceResponse<string?>( token, true, $"Successfully created new session for user {userId}." );
    }
    public async Task<ServiceResponse<string?>> GetExistingSession( int userId, string sessionToken, string ipAddress )
    {
        UserSession? session = await _sessionRepository.GetSession( userId, ipAddress );

        if ( session == null )
            return new ServiceResponse<string?>( null, false, $"Failed to get session for User {userId}!" );

        if ( !VerifySessionToken( sessionToken, session.Hash, session.Salt ) )
            return new ServiceResponse<string?>( null, false, $"Failed to validate session for User {userId}!" );

        session.LastActivityDate = DateTime.Now;

        bool updated = await _sessionRepository.UpdateSession( session );
        
        return !updated 
            ? new ServiceResponse<string?>( null, false, $"Failed to update session for User {userId}!" ) 
            : new ServiceResponse<string?>( sessionToken, true, $"Successfully retrieved session for user {userId}." );
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