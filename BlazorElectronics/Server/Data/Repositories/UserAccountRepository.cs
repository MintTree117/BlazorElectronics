using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Server.Core.Models.Users;
using Dapper;

namespace BlazorElectronics.Server.Data.Repositories;

public class UserAccountRepository : DapperRepository, IUserRepository
{
    const string PROCEDURE_GET_ALL_IDS = "Get_UserIds";
    const string PROCEDURE_GET_BY_ID = "Get_UserAccountById";
    const string PROCEDURE_GET_BY_USERNAME = "Get_UserAccountByUsername";
    const string PROCEDURE_GET_BY_EMAIL = "Get_UserAccountByEmail";
    const string PROCEDURE_GET_BY_NAME_OR_EMAIL = "Get_UserAccountByNameOrEmail";
    const string PROCEDURE_GET_USER_EXISTS = "Get_UserAccountExists";
    const string PROCEDURE_INSERT = "Insert_UserAccount";
    const string PROCEDURE_UPDATE_PASSWORD = "Update_UserAccountPassword";
    const string PROCEDURE_INSERT_VERIFICATION_CODE = "Insert_VerificationToken";
    const string PROCEDURE_UPDATE_VERIFICATION_TOKEN = "Update_VerificationToken";
    const string PROCEDURE_UPDATE_ACCOUNT_STATUS = "Update_UserAccountStatus";

    public UserAccountRepository( DapperContext dapperContext ) : base( dapperContext ) { }

    public async Task<IEnumerable<int>?> GetAllIds()
    {
        return await TryQueryAsync( Query<int>, null, PROCEDURE_GET_ALL_IDS );
    }
    public async Task<UserModel?> GetById( int id )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_ID, id );
        
        return await TryQueryAsync( QuerySingleOrDefault<UserModel?>, p, PROCEDURE_GET_BY_ID );
    }
    public async Task<UserModel?> GetByUsername( string username )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_NAME, username );
        
        return await TryQueryAsync( QuerySingleOrDefault<UserModel?>, p, PROCEDURE_GET_BY_USERNAME );
    }
    public async Task<UserModel?> GetByEmail( string email )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_EMAIL, email );
        
        return await TryQueryAsync( QuerySingleOrDefault<UserModel?>, p, PROCEDURE_GET_BY_EMAIL );
    }
    public async Task<UserModel?> GetByEmailOrUsername( string emailOrUsername )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_NAME_OR_EMAIL, emailOrUsername );
        
        return await TryQueryAsync( QuerySingleOrDefault<UserModel?>, p, PROCEDURE_GET_BY_NAME_OR_EMAIL );
    }
    public async Task<UserExists?> GetUserExists( string username, string email )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_NAME, username );
        p.Add( PARAM_USER_EMAIL, email );
        
        return await TryQueryAsync( QuerySingleOrDefault<UserExists?>, p, PROCEDURE_GET_USER_EXISTS );
    }
    public async Task<UserModel?> InsertUser( string username, string email, string? phone, byte[] hash, byte[] salt )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_NAME, username );
        p.Add( PARAM_USER_EMAIL, email );
        p.Add( PARAM_USER_PHONE, phone );
        p.Add( PARAM_USER_PASSWORD_HASH, hash );
        p.Add( PARAM_USER_PASSWORD_SALT, salt );

        return await TryQueryTransactionAsync( QuerySingleOrDefaultTransaction<UserModel?>, p, PROCEDURE_INSERT );
    }
    public async Task<bool> UpdatePassword( int id, byte[] hash, byte[] salt )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_ID, id );
        p.Add( PARAM_USER_PASSWORD_HASH, hash );
        p.Add( PARAM_USER_PASSWORD_SALT, salt );
        
        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_UPDATE_PASSWORD );
    }
    public async Task<bool> InsertVerificationCode( int userId, string code )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_ID, userId );
        p.Add( PARAM_USER_VERIFICATION_TOKEN, code );
        
        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_INSERT_VERIFICATION_CODE );
    }
    public async Task<int> Update_VerificationToken( string token )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_VERIFICATION_TOKEN, token );
        
        return await TryQueryTransactionAsync( QuerySingleOrDefaultTransaction<int>, p, PROCEDURE_UPDATE_VERIFICATION_TOKEN );
    }
    public async Task<bool> Update_UserAccountStatus( int userId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_USER_ID, userId );

        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_UPDATE_ACCOUNT_STATUS );
    }
}