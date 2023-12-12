using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Server.Core.Models.Users;
using Dapper;

namespace BlazorElectronics.Server.Data.Repositories;

public class UserRepository : DapperRepository, IUserRepository
{
    const string PROCEDURE_GET_ALL_IDS = "Get_UserIds";
    const string PROCEDURE_GET_BY_ID = "Get_UserAccountById";
    const string PROCEDURE_GET_BY_USERNAME = "Get_UserAccountByUsername";
    const string PROCEDURE_GET_BY_EMAIL = "Get_UserAccountByEmail";
    const string PROCEDURE_GET_BY_NAME_OR_EMAIL = "Get_UserAccountByNameOrEmail";
    const string PROCEDURE_GET_USER_EXISTS = "Get_UserAccountExists";
    const string PROCEDURE_INSERT = "Insert_UserAccount";
    const string PROCEDURE_UPDATE_PASSWORD = "Update_UserAccountPassword";

    public UserRepository( DapperContext dapperContext ) : base( dapperContext ) { }

    public async Task<IEnumerable<int>?> GetAllIds()
    {
        return await TryQueryAsync( Query<int>, null, PROCEDURE_GET_ALL_IDS );
    }
    public async Task<User?> GetById( int id )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_ID, id );
        return await TryQueryAsync( QuerySingleOrDefault<User?>, p, PROCEDURE_GET_BY_ID );
    }
    public async Task<User?> GetByUsername( string username )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_NAME, username );
        return await TryQueryAsync( QuerySingleOrDefault<User?>, p, PROCEDURE_GET_BY_USERNAME );
    }
    public async Task<User?> GetByEmail( string email )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_EMAIL, email );
        return await TryQueryAsync( QuerySingleOrDefault<User?>, p, PROCEDURE_GET_BY_EMAIL );
    }
    public async Task<User?> GetByEmailOrUsername( string emailOrUsername )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_NAME_OR_EMAIL, emailOrUsername );
        return await TryQueryAsync( QuerySingleOrDefault<User?>, p, PROCEDURE_GET_BY_NAME_OR_EMAIL );
    }
    public async Task<UserExists?> GetUserExists( string username, string email )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_NAME, username );
        p.Add( PARAM_USER_EMAIL, email );
        return await TryQueryAsync( QuerySingleOrDefault<UserExists?>, p, PROCEDURE_GET_USER_EXISTS );
    }
    public async Task<User?> InsertUser( string username, string email, string? phone, byte[] hash, byte[] salt )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_NAME, username );
        p.Add( PARAM_USER_EMAIL, email );
        p.Add( PARAM_USER_PHONE, phone );
        p.Add( PARAM_USER_PASSWORD_HASH, hash );
        p.Add( PARAM_USER_PASSWORD_SALT, salt );

        return await TryQueryTransactionAsync( QuerySingleOrDefaultTransaction<User?>, p, PROCEDURE_INSERT );
    }
    public async Task<bool> UpdatePassword( int id, byte[] hash, byte[] salt )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_ID, id );
        p.Add( PARAM_USER_PASSWORD_HASH, hash );
        p.Add( PARAM_USER_PASSWORD_SALT, salt );

        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_UPDATE_PASSWORD );
    }
}