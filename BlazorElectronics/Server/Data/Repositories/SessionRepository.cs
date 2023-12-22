using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Server.Core.Models.Sessions;
using BlazorElectronics.Server.Core.Models.Users;
using Dapper;

namespace BlazorElectronics.Server.Data.Repositories;

public class SessionRepository : DapperRepository, ISessionRepository
{
    const string PROCEDURE_GET = "Get_Session";
    const string PROCEDURE_GET_VALIDATION = "Get_SessionValidation";
    const string PROCEDURE_INSERT = "Insert_Session";
    const string PROCEDURE_DELETE = "Delete_Session";

    public SessionRepository( DapperContext dapperContext )
        : base( dapperContext ) { }

    public async Task<UserSession?> InsertSession( int userId, byte[] sessionHash, byte[] sessionSalt, UserDeviceInfoDto? deviceInfo )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_ID, userId );
        p.Add( PARAM_SESSION_IP_ADDRESS, deviceInfo?.IpAddress );
        p.Add( PARAM_SESSION_HASH, sessionHash );
        p.Add( PARAM_SESSION_SALT, sessionSalt );

        return await TryQueryTransactionAsync( QuerySingleOrDefaultTransaction<UserSession?>, p, PROCEDURE_INSERT );
    }
    public async Task<bool> DeleteSession( int sessionId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_SESSION_ID, sessionId );
        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_DELETE );
    }
    public async Task<UserSession?> GetSession( int sessionId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_SESSION_ID, sessionId );
        return await TryQueryAsync( QuerySingleOrDefault<UserSession?>, p, PROCEDURE_GET );
    }
    public async Task<SessionValidationModel?> GetSessionValidation( int sessionId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_SESSION_ID, sessionId );

        return await TryQueryAsync( QuerySingleOrDefault<SessionValidationModel?>, p, PROCEDURE_GET_VALIDATION );
    }
}