using BlazorElectronics.Server.Core.Models.Users;
using BlazorElectronics.Shared.Users;

namespace BlazorElectronics.Server.Core.Interfaces;

public interface IUserAccountRepository
{
    Task<IEnumerable<int>?> GetAllIds();
    Task<UserModel?> GetById( int id );
    Task<UserModel?> GetByUsername( string username );
    Task<UserModel?> GetByEmail( string email );
    Task<UserModel?> GetByEmailOrUsername( string emailOrUsername );
    Task<UserExists?> GetUserExists( string username, string email );
    Task<AccountDetailsDto?> GetAccountDetails( int userId );
    Task<UserModel?> InsertUser( string username, string email, string? phone, byte[] hash, byte[] salt );
    Task<UserValidationModel?> GetValidation( int userId );
    Task<AccountDetailsDto?> UpdateAccountDetails( int userId, AccountDetailsDto dto );
    Task<bool> UpdatePassword( int id, byte[] hash, byte[] salt );
    Task<bool> InsertVerificationCode( int userId, string code );
    Task<int> Update_VerificationToken( string token );
    Task<bool> Update_UserAccountStatus( int userId );
    Task<int> GetIdByEmail( string email );
}
