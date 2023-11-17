using System.Data;
using System.Data.Common;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Dtos.Users;
using BlazorElectronics.Server.Models.Users;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Sessions;

public class SessionRepository : DapperRepository, ISessionRepository
{
    const string PROCEDURE_ADD_SESSION = "Add_UserSession";
    const string PROCEDURE_GET_SESSION = "Get_UserSession";

    public SessionRepository( DapperContext dapperContext )
        : base( dapperContext ) { }

    public async Task<UserSession?> AddSession( int userId, byte[] sessionHash, byte[] sessionSalt, UserDeviceInfoDto? deviceInfo )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_USER_ID, userId );
        parameters.Add( PARAM_SESSION_IP_ADDRESS, deviceInfo?.IpAddress );
        parameters.Add( PARAM_SESSION_SALT, sessionHash );
        parameters.Add( PARAM_SESSION_HASH, sessionSalt );

        return await TryQueryTransactionAsync( AddSessionQuery, parameters );
    }
    public async Task<UserSession?> GetSession( int sessionId )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_SESSION_ID, sessionId );

        return await TryQueryAsync( GetSessionQuery, parameters );
    }
    
    static async Task<UserSession?> AddSessionQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        return await connection.QuerySingleOrDefaultAsync<UserSession>( PROCEDURE_ADD_SESSION, dynamicParams, transaction, commandType: CommandType.StoredProcedure );
    }
    static async Task<UserSession?> GetSessionQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        return await connection.QuerySingleOrDefaultAsync<UserSession>( PROCEDURE_GET_SESSION, dynamicParams, commandType: CommandType.StoredProcedure );
    }
}