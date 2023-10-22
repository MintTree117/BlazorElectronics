using System.Data;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Users;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Users;

public class UserRepository : DapperRepository<User>, IUserRepository
{
    const string STORED_PROCEDURE_GET_ALL_USERS = "Get_AllUsers";
    const string STORED_PROCEDURE_GET_USER_BY_ID = "Get_UserById";
    const string STORED_PROCEDURE_GET_USER_BY_USERNAME = "Get_UserByUsername";
    const string STORED_PROCEDURE_GET_USER_BY_EMAIL = "Get_UserByEmail";
    const string STORED_PROCEDURE_GET_USER_BY_NAME_OR_EMAIL = "Get_UserByNameOrEmail";
    const string STORED_PROCEDURE_INSERT_NEW_USER = "Insert_NewUser";

    const string QUERY_PARAM_USER_NAME = "@Username";
    const string QUERY_PARAM_USER_EMAIL = "@Email";
    const string QUERY_PARAM_USER_HASH = "@Hash";
    const string QUERY_PARAM_USER_SALT = "@Salt";
    const string QUERY_PARAM_USER_DATE = "@Date";
    
    public UserRepository( DapperContext dapperContext ) : base( dapperContext ) { }
    
    public override async Task<IEnumerable<User>?> GetAll()
    {
        throw new NotImplementedException();
    }
    public override async Task<User?> GetById( int id )
    {
        throw new NotImplementedException();
    }
    public override async Task Insert( User item )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( QUERY_PARAM_USER_NAME, item.Username );
        dynamicParams.Add( QUERY_PARAM_USER_EMAIL, item.Email );
        dynamicParams.Add( QUERY_PARAM_USER_HASH, item.PasswordHash );
        dynamicParams.Add( QUERY_PARAM_USER_SALT, item.PasswordSalt );
        dynamicParams.Add( QUERY_PARAM_USER_DATE, item.DateCreated );
        
        await using SqlConnection connection = await _dbContext.GetOpenConnection();
        await connection.ExecuteAsync( STORED_PROCEDURE_INSERT_NEW_USER, dynamicParams, commandType: CommandType.StoredProcedure ).ConfigureAwait( false );
    }
    public async Task<User?> GetByUsername( string username )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( QUERY_PARAM_USER_NAME, username );

        await using SqlConnection connection = await _dbContext.GetOpenConnection();
        
        try
        {
            return await connection.QueryFirstAsync<User>( STORED_PROCEDURE_GET_USER_BY_USERNAME, dynamicParams, commandType: CommandType.StoredProcedure );
        }
        catch
        {
            return null;
        }
    }
    public async Task<User?> GetByEmail( string email )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( QUERY_PARAM_USER_EMAIL, email );

        await using SqlConnection connection = await _dbContext.GetOpenConnection();

        try
        {
            return await connection.QueryFirstAsync<User>( STORED_PROCEDURE_GET_USER_BY_EMAIL, dynamicParams, commandType: CommandType.StoredProcedure );
        }
        catch
        {
            return null;
        }
    }
    public async Task<UserExists> CheckIfUserExists( string username, string email )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( QUERY_PARAM_USER_NAME, username );
        dynamicParams.Add( QUERY_PARAM_USER_EMAIL, email );

        await using SqlConnection connection = await _dbContext.GetOpenConnection();

        User? user = null;

        try
        {
            user = await connection.QueryFirstAsync<User>( STORED_PROCEDURE_GET_USER_BY_NAME_OR_EMAIL, dynamicParams, commandType: CommandType.StoredProcedure );
        }
        catch
        {
            return new UserExists();
        }

        return new UserExists {
            UsernameExists = user.Username == username,
            EmailExists = user.Email == email
        };
    }
}