using System.Data;
using System.Data.Common;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Users;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Users;

public class UserRepository : DapperRepository, IUserRepository
{
    const string STORED_PROCEDURE_GET_USER_BY_ID = "Get_UserById";
    const string STORED_PROCEDURE_GET_USER_BY_USERNAME = "Get_UserByUsername";
    const string STORED_PROCEDURE_GET_USER_BY_EMAIL = "Get_UserByEmail";
    const string STORED_PROCEDURE_GET_USER_BY_NAME_OR_EMAIL = "Get_UserByNameOrEmail";
    const string STORED_PROCEDURE_CREATE_USER = "Create_User";
    const string STORED_PROCEDURE_UPDATE_PASSWORD = "Update_UserPassword";
    
    const string QUERY_PARAM_USER_ID = "@UserId";
    const string QUERY_PARAM_USER_NAME = "@Username";
    const string QUERY_PARAM_USER_EMAIL = "@Email";
    const string QUERY_PARAM_USER_HASH = "@Hash";
    const string QUERY_PARAM_USER_SALT = "@Salt";
    
    public UserRepository( DapperContext dapperContext )
        : base( dapperContext ) { }
    
    public async Task<User?> GetById( int id )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( QUERY_PARAM_USER_ID, id );
        
        return await TryQueryAsync( GetByIdQuery, dynamicParams );
    }
    public async Task<User?> GetByUsername( string username )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( QUERY_PARAM_USER_NAME, username );

        return await TryQueryAsync( GetByUsernameQuery, dynamicParams );
    }
    public async Task<User?> GetByEmail( string email )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( QUERY_PARAM_USER_EMAIL, email );

        return await TryQueryAsync( GetByEmailQuery, dynamicParams );
    }
    public async Task<UserExists?> CheckIfUserExists( string username, string email )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( QUERY_PARAM_USER_NAME, username );
        dynamicParams.Add( QUERY_PARAM_USER_EMAIL, email );

        return await TryQueryAsync( CheckIfUserExistsQuery, dynamicParams );
    }
    public async Task<User?> AddUser( User user )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( QUERY_PARAM_USER_NAME, user.Username );
        dynamicParams.Add( QUERY_PARAM_USER_EMAIL, user.Email );
        dynamicParams.Add( QUERY_PARAM_USER_HASH, user.PasswordHash );
        dynamicParams.Add( QUERY_PARAM_USER_SALT, user.PasswordSalt );

        return await TryQueryTransactionAsync( AddUserQuery, dynamicParams );
    }
    public async Task<bool> UpdateUserPassword( int id, byte[] hash, byte[] salt )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( QUERY_PARAM_USER_ID, id );
        dynamicParams.Add( QUERY_PARAM_USER_HASH, hash );
        dynamicParams.Add( QUERY_PARAM_USER_SALT, salt );

        return await TryQueryTransactionAsync( UpdateUserPasswordQuery, dynamicParams );
    }
    
    static async Task<User?> GetByIdQuery( SqlConnection connection, DynamicParameters? dynamicParams )
    {
        return await connection.QuerySingleAsync<User?>( STORED_PROCEDURE_GET_USER_BY_ID, dynamicParams, commandType: CommandType.StoredProcedure );
    }
    static async Task<User?> GetByUsernameQuery( SqlConnection connection, DynamicParameters? dynamicParams )
    {
        return await connection.QuerySingleAsync<User?>( STORED_PROCEDURE_GET_USER_BY_USERNAME, dynamicParams, commandType: CommandType.StoredProcedure );
    }
    static async Task<User?> GetByEmailQuery( SqlConnection connection, DynamicParameters? dynamicParams )
    {
        return await connection.QuerySingleAsync<User?>( STORED_PROCEDURE_GET_USER_BY_EMAIL, dynamicParams, commandType: CommandType.StoredProcedure );
    }
    static async Task<UserExists?> CheckIfUserExistsQuery( SqlConnection connection, DynamicParameters? dynamicParams )
    {
        return await connection.QuerySingleAsync<UserExists?>( STORED_PROCEDURE_GET_USER_BY_NAME_OR_EMAIL, dynamicParams, commandType: CommandType.StoredProcedure );
    }
    static async Task<User?> AddUserQuery( SqlConnection connection, DbTransaction transaction, DynamicParameters? dynamicParams )
    {
        return await connection.QuerySingleAsync<User?>( STORED_PROCEDURE_CREATE_USER, dynamicParams, commandType: CommandType.StoredProcedure );
    }
    static async Task<bool> UpdateUserPasswordQuery( SqlConnection connection, DbTransaction transaction, DynamicParameters? dynamicParams )
    {
        int? result = await connection.ExecuteAsync( STORED_PROCEDURE_UPDATE_PASSWORD, dynamicParams, commandType: CommandType.StoredProcedure );
        return result is > 0;
    }
}