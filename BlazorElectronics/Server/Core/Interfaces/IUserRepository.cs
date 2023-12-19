using BlazorElectronics.Server.Core.Models.Users;

namespace BlazorElectronics.Server.Core.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<int>?> GetAllIds();
    Task<UserModel?> GetById( int id );
    Task<UserModel?> GetByUsername( string username );
    Task<UserModel?> GetByEmail( string email );
    Task<UserModel?> GetByEmailOrUsername( string emailOrUsername );
    Task<UserExists?> GetUserExists( string username, string email );
    Task<UserModel?> InsertUser( string username, string email, string? phone, byte[] hash, byte[] salt );
    Task<bool> UpdatePassword( int id, byte[] hash, byte[] salt );
    Task<bool> InsertVerificationCode( int userId, string code );
    Task<int> Update_VerificationToken( string token );
    Task<bool> Update_UserAccountStatus( int userId );
}
