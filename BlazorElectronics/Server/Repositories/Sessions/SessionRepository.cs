using System.Data;
using System.Data.Common;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Users;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Sessions;

public class SessionRepository : DapperRepository, ISessionRepository
{
    const string STORED_PROCEDURE_GET_SESSION = "Get_Session";
    const string STORED_PROCEDURE_CREATE_SESSION = "Create_Session";
    const string STORED_PROCEDURE_UPDATE_SESSION = "Update_Session";

    const string QUERY_PARAM_USER_ID = "@UserId";
    const string QUERY_PARAM_SESSION_DATE = "@Date";
    const string QUERY_PARAM_DATE_ACTIVE = "@LastActive";
    const string QUERY_PARAM_IS_ACTIVE = "@IsActive";
    const string QUERY_PARAM_IP_ADDRESS = "@IpAddress";
    const string QUERY_PARAM_SESSION_SALT = "@Salt";
    const string QUERY_PARAM_SESSION_HASH = "@Hash";

    public SessionRepository( DapperContext dapperContext )
        : base( dapperContext ) { }
    
    public async Task<UserSession?> GetSession( int userId, string ipAddress )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( QUERY_PARAM_USER_ID, userId );
        dynamicParams.Add( QUERY_PARAM_IP_ADDRESS, ipAddress );

        return await TryQueryAsync( GetSessionQuery, dynamicParams );
    }
    public async Task<UserSession?> CreateSession( UserSession session )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( QUERY_PARAM_USER_ID, session.UserId );
        dynamicParams.Add( QUERY_PARAM_SESSION_DATE, session.DateCreated );
        dynamicParams.Add( QUERY_PARAM_DATE_ACTIVE, session.DateCreated );
        dynamicParams.Add( QUERY_PARAM_IS_ACTIVE, session.IsActive );
        dynamicParams.Add( QUERY_PARAM_IP_ADDRESS, session.IpAddress );
        dynamicParams.Add( QUERY_PARAM_SESSION_SALT, session.Salt );
        dynamicParams.Add( QUERY_PARAM_SESSION_HASH, session.Hash );

        return await TryQueryTransactionAsync( CreateSessionQuery, dynamicParams );
    }
    public async Task<bool> UpdateSession( UserSession update )
    {
        return await TryQueryTransactionAsync( UpdateSessionQuery, null );
    }

    static async Task<UserSession?> GetSessionQuery( SqlConnection connection, DynamicParameters? dynamicParams )
    {
        return await connection.QuerySingleAsync<UserSession?>( STORED_PROCEDURE_GET_SESSION, dynamicParams, commandType: CommandType.StoredProcedure );
    }
    static async Task<UserSession?> CreateSessionQuery( SqlConnection connection, DbTransaction transaction, DynamicParameters? dynamicParams )
    {
        return await connection.QuerySingleAsync<UserSession?>( STORED_PROCEDURE_CREATE_SESSION, dynamicParams, commandType: CommandType.StoredProcedure );
    }
    static async Task<bool> UpdateSessionQuery( SqlConnection connection, DbTransaction transaction, DynamicParameters? dynamicParams )
    {
        var result = await connection.QuerySingleAsync<UserSession?>( STORED_PROCEDURE_UPDATE_SESSION, dynamicParams, commandType: CommandType.StoredProcedure );
        return result != null;
    }
}