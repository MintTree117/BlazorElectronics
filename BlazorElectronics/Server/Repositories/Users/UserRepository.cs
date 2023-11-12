using System.Data;
using System.Data.Common;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Users;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Users;

public class UserRepository : DapperRepository, IUserRepository
{
    const string PROCEDURE_GET_USER_BY_ID = "Get_UserById";
    const string PROCEDURE_GET_USER_BY_USERNAME = "Get_UserByUsername";
    const string PROCEDURE_GET_USER_BY_EMAIL = "Get_UserByEmail";
    const string PROCEDURE_GET_USER_BY_NAME_OR_EMAIL = "Get_UserByNameOrEmail";
    const string PROCEDURE_GET_USER_EXISTS = "Get_UserExists";
    const string PROCEDURE_ADD_USER = "Create_User";
    const string PROCEDURE_UPDATE_PASSWORD = "Update_UserPassword";

    public UserRepository( DapperContext dapperContext ) : base( dapperContext ) { }
    
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
        parameters.Add( PARAM_USER_EMAIL_OR_NAME, emailOrUsername );

        return await TryQueryAsync( GetByEmailOrUsernameQuery, parameters );
    }
    public async Task<UserExists?> GetUserExists( string username, string email )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_USER_NAME, username );
        parameters.Add( PARAM_USER_EMAIL, email );

        return await TryQueryAsync( GetUserExistsQuery, parameters );
    }
    public async Task<User?> AddUser( string username, string email, int? phone, byte[] hash, byte[] salt )
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
    
    static async Task<User?> GetByIdQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        return await connection.QuerySingleAsync<User?>( PROCEDURE_GET_USER_BY_ID, dynamicParams, commandType: CommandType.StoredProcedure );
    }
    static async Task<User?> GetByUsernameQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        return await connection.QuerySingleAsync<User?>( PROCEDURE_GET_USER_BY_USERNAME, dynamicParams, commandType: CommandType.StoredProcedure );
    }
    static async Task<User?> GetByEmailQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        return await connection.QuerySingleAsync<User?>( PROCEDURE_GET_USER_BY_EMAIL, dynamicParams, commandType: CommandType.StoredProcedure );
    }
    static async Task<User?> GetByEmailOrUsernameQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        return await connection.QuerySingleAsync<User?>( PROCEDURE_GET_USER_BY_NAME_OR_EMAIL, dynamicParams, commandType: CommandType.StoredProcedure );
    }
    static async Task<UserExists?> GetUserExistsQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        return await connection.QuerySingleAsync<UserExists?>( PROCEDURE_GET_USER_EXISTS, dynamicParams, commandType: CommandType.StoredProcedure );
    }
    static async Task<User?> AddUserQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        return await connection.QuerySingleAsync<User?>( PROCEDURE_ADD_USER, dynamicParams, commandType: CommandType.StoredProcedure );
    }
    static async Task<bool> UpdateUserPasswordQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        int? result = await connection.ExecuteAsync( PROCEDURE_UPDATE_PASSWORD, dynamicParams, commandType: CommandType.StoredProcedure );
        return result is > 0;
    }
}