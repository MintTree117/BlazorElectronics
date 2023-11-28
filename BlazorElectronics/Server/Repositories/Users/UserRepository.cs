using System.Data;
using System.Data.Common;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Users;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Users;

public class UserRepository : DapperRepository, IUserRepository
{
    const string PROCEDURE_GET_ALL_IDS = "Get_UserIds";
    const string PROCEDURE_GET_USER_BY_ID = "Get_UserAccountById";
    const string PROCEDURE_GET_USER_BY_USERNAME = "Get_UserAccountByUsername";
    const string PROCEDURE_GET_USER_BY_EMAIL = "Get_UserAccountByEmail";
    const string PROCEDURE_GET_USER_BY_NAME_OR_EMAIL = "Get_UserAccountByNameOrEmail";
    const string PROCEDURE_GET_USER_EXISTS = "Get_UserAccountExists";
    const string PROCEDURE_INSERT_USER = "Insert_UserAccount";
    const string PROCEDURE_UPDATE_PASSWORD = "Update_UserAccountPassword";

    public UserRepository( DapperContext dapperContext ) : base( dapperContext ) { }

    public async Task<List<int>?> GetAllIds()
    {
        return await TryQueryAsync( GetAllIdsQuery );
    }
    public async Task<User?> GetById( int id )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_USER_ID, id );

        return await TryQueryAsync( GetByIdQuery, parameters );
    }
    public async Task<User?> GetByUsername( string username )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_USER_NAME, username );

        return await TryQueryAsync( GetByUsernameQuery, parameters );
    }
    public async Task<User?> GetByEmail( string email )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_USER_EMAIL, email );

        return await TryQueryAsync( GetByEmailQuery, parameters );
    }
    public async Task<User?> GetByEmailOrUsername( string emailOrUsername )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_USER_NAME_OR_EMAIL, emailOrUsername );

        return await TryQueryAsync( GetByEmailOrUsernameQuery, parameters );
    }
    public async Task<UserExists?> GetUserExists( string username, string email )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_USER_NAME, username );
        parameters.Add( PARAM_USER_EMAIL, email );

        return await TryQueryAsync( GetUserExistsQuery, parameters );
    }
    public async Task<User?> InsertUser( string username, string email, string? phone, byte[] hash, byte[] salt )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_USER_NAME, username );
        parameters.Add( PARAM_USER_EMAIL, email );
        parameters.Add( PARAM_USER_PHONE, phone );
        parameters.Add( PARAM_USER_PASSWORD_HASH, hash );
        parameters.Add( PARAM_USER_PASSWORD_SALT, salt );

        return await TryQueryTransactionAsync( AddUserQuery, parameters );
    }
    public async Task<bool> UpdatePassword( int id, byte[] hash, byte[] salt )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_USER_ID, id );
        parameters.Add( PARAM_USER_PASSWORD_HASH, hash );
        parameters.Add( PARAM_USER_PASSWORD_SALT, salt );

        return await TryQueryTransactionAsync( UpdateUserPasswordQuery, parameters );
    }

    static async Task<List<int>?> GetAllIdsQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        IEnumerable<int>? ids = await connection.QueryAsync<int>( PROCEDURE_GET_ALL_IDS, dynamicParams, commandType: CommandType.StoredProcedure );
        return ids.ToList();
    }
    static async Task<User?> GetByIdQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        return await connection.QuerySingleOrDefaultAsync<User>( PROCEDURE_GET_USER_BY_ID, dynamicParams, commandType: CommandType.StoredProcedure );
    }
    static async Task<User?> GetByUsernameQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        return await connection.QuerySingleOrDefaultAsync<User>( PROCEDURE_GET_USER_BY_USERNAME, dynamicParams, commandType: CommandType.StoredProcedure );
    }
    static async Task<User?> GetByEmailQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        return await connection.QuerySingleOrDefaultAsync<User>( PROCEDURE_GET_USER_BY_EMAIL, dynamicParams, commandType: CommandType.StoredProcedure );
    }
    static async Task<User?> GetByEmailOrUsernameQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        return await connection.QuerySingleOrDefaultAsync<User>( PROCEDURE_GET_USER_BY_NAME_OR_EMAIL, dynamicParams, commandType: CommandType.StoredProcedure );
    }
    static async Task<UserExists?> GetUserExistsQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        return await connection.QuerySingleOrDefaultAsync<UserExists>( PROCEDURE_GET_USER_EXISTS, dynamicParams, commandType: CommandType.StoredProcedure );
    }
    static async Task<User?> AddUserQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        return await connection.QuerySingleOrDefaultAsync<User>( PROCEDURE_INSERT_USER, dynamicParams, transaction, commandType: CommandType.StoredProcedure );
    }
    static async Task<bool> UpdateUserPasswordQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        int? result = await connection.ExecuteAsync( PROCEDURE_UPDATE_PASSWORD, dynamicParams, transaction, commandType: CommandType.StoredProcedure );
        return result is > 0;
    }
}