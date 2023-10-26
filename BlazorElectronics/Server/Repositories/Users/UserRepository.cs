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
    const string STORED_PROCEDURE_ADD_USER = "Add_User";
    const string STORED_PROCEDURE_UPDATE_PASSWORD = "Update_UserPassword";

    const string QUERY_PARAM_USER_ID = "@Id";
    const string QUERY_PARAM_USER_NAME = "@Username";
    const string QUERY_PARAM_USER_EMAIL = "@Email";
    const string QUERY_PARAM_USER_HASH = "@Hash";
    const string QUERY_PARAM_USER_SALT = "@Salt";
    const string QUERY_PARAM_USER_DATE = "@Date";

    public UserRepository( DapperContext dapperContext ) : base( dapperContext ) { }

    public async Task<User?> GetById( int id )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( QUERY_PARAM_USER_ID, id );
        
        try
        {
            await using SqlConnection? connection = await _dbContext.GetOpenConnection();
            return await connection.QuerySingleAsync<User>( STORED_PROCEDURE_GET_USER_BY_ID, dynamicParams, commandType: CommandType.StoredProcedure );
        }
        catch( SqlException e )
        {
            throw new RepositoryException( e.Message, e );
        }
        catch ( Exception e )
        {
            throw new RepositoryException( e.Message, e );
        }
    }
    public async Task<User?> GetByUsername( string username )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( QUERY_PARAM_USER_NAME, username );

        try
        {
            await using SqlConnection? connection = await _dbContext.GetOpenConnection();
            return await connection.QuerySingleAsync<User>( STORED_PROCEDURE_GET_USER_BY_USERNAME, dynamicParams, commandType: CommandType.StoredProcedure );
        }
        catch ( SqlException e )
        {
            throw new RepositoryException( e.Message, e );
        }
        catch ( Exception e )
        {
            throw new RepositoryException( e.Message, e );
        }
    }
    public async Task<User?> GetByEmail( string email )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( QUERY_PARAM_USER_EMAIL, email );

        try
        {
            await using SqlConnection? connection = await _dbContext.GetOpenConnection();
            return await connection.QuerySingleAsync<User>( STORED_PROCEDURE_GET_USER_BY_EMAIL, dynamicParams, commandType: CommandType.StoredProcedure );
        }
        catch ( SqlException e )
        {
            throw new RepositoryException( e.Message, e );
        }
        catch ( Exception e )
        {
            throw new RepositoryException( e.Message, e );
        }
    }
    public async Task<UserExists?> CheckIfUserExists( string username, string email )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( QUERY_PARAM_USER_NAME, username );
        dynamicParams.Add( QUERY_PARAM_USER_EMAIL, email );
        
        try
        {
            await using SqlConnection? connection = await _dbContext.GetOpenConnection();
            var user = await connection.QuerySingleAsync<User>( STORED_PROCEDURE_GET_USER_BY_NAME_OR_EMAIL, dynamicParams, commandType: CommandType.StoredProcedure );
            return new UserExists {
                UsernameExists = user.Username == username,
                EmailExists = user.Email == email
            };
        }
        catch ( SqlException e )
        {
            throw new RepositoryException( e.Message, e );
        }
        catch ( Exception e )
        {
            throw new RepositoryException( e.Message, e );
        }
    }
    public async Task<bool> UpdateUserPassword( int id, byte[] hash, byte[] salt )
    {
        SqlConnection? connection = null;
        DbTransaction? transaction = null;
        
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( QUERY_PARAM_USER_ID, id );
        dynamicParams.Add( QUERY_PARAM_USER_HASH, hash );
        dynamicParams.Add( QUERY_PARAM_USER_SALT, salt );

        try
        {
            connection = await _dbContext.GetOpenConnection();
            transaction = await connection!.BeginTransactionAsync();
        }
        catch ( SqlException e )
        {
            await HandleConnectionTransactionDisposal( connection, transaction );
            throw GetRepositoryException( e );
        }
        catch ( Exception e )
        {
            await HandleConnectionTransactionDisposal( connection, transaction );
            throw GetRepositoryException( e );
        }
        
        try
        {
            await connection.ExecuteAsync( STORED_PROCEDURE_UPDATE_PASSWORD, dynamicParams, commandType: CommandType.StoredProcedure ).ConfigureAwait( false );
            await transaction.CommitAsync();
            await transaction.DisposeAsync();
            await connection.CloseAsync();
            return true;
        }
        catch ( SqlException e )
        {
            await HandleConnectionTransactionRollbackDisposal( connection, transaction );
            throw GetRepositoryException( e );
        }
        catch ( Exception e )
        {
            await HandleConnectionTransactionRollbackDisposal( connection, transaction );
            throw GetRepositoryException( e );
        }
    }
    public async Task<User?> AddUser( User user )
    {
        SqlConnection? connection = null;
        DbTransaction? transaction = null;
        
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( QUERY_PARAM_USER_NAME, user.Username );
        dynamicParams.Add( QUERY_PARAM_USER_EMAIL, user.Email );
        dynamicParams.Add( QUERY_PARAM_USER_HASH, user.PasswordHash );
        dynamicParams.Add( QUERY_PARAM_USER_SALT, user.PasswordSalt );
        dynamicParams.Add( QUERY_PARAM_USER_DATE, user.DateCreated );

        try
        {
            connection = await _dbContext.GetOpenConnection();
            transaction = await connection!.BeginTransactionAsync();
        }
        catch ( SqlException e )
        {
            await HandleConnectionTransactionDisposal( connection, transaction );
            throw GetRepositoryException( e );
        }
        catch ( Exception e )
        {
            await HandleConnectionTransactionDisposal( connection, transaction );
            throw GetRepositoryException( e );
        }
        
        try
        {
            User? insertedUser = await connection.QuerySingleAsync<User>( STORED_PROCEDURE_ADD_USER, dynamicParams, commandType: CommandType.StoredProcedure ).ConfigureAwait( false );
            await transaction.CommitAsync();
            await transaction.DisposeAsync();
            await connection.CloseAsync();
            return insertedUser;
        }
        catch ( SqlException e )
        {
            await HandleConnectionTransactionRollbackDisposal( connection, transaction );
            throw GetRepositoryException( e );
        }
        catch ( Exception e )
        {
            await HandleConnectionTransactionRollbackDisposal( connection, transaction );
            throw GetRepositoryException( e );
        }
    }
}