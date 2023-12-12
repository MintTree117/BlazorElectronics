using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Server.Core.Models.Users;
using Dapper;

namespace BlazorElectronics.Server.Data.Repositories;

public class SessionRepository : DapperRepository, ISessionRepository
{
    const string PROCEDURE_GET = "Get_UserSession";
    const string PROCEDURE_INSERT = "Insert_UserSession";
    const string PROCEDURE_DELETE = "Delete_UserSession";

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
}