using BlazorElectronics.Server.Core.Models.Users;

namespace BlazorElectronics.Server.Core.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<int>?> GetAllIds();
    Task<User?> GetById( int id );
    Task<User?> GetByUsername( string username );
    Task<User?> GetByEmail( string email );
    Task<User?> GetByEmailOrUsername( string emailOrUsername );
    Task<UserExists?> GetUserExists( string username, string email );
    Task<User?> InsertUser( string username, string email, string? phone, byte[] hash, byte[] salt );
    Task<bool> UpdatePassword( int id, byte[] hash, byte[] salt );
    Task<bool> InsertVerificationCode( int userId, string code );
    Task<int> Update_VerificationToken( string token );
    Task<bool> Update_UserAccountStatus( int userId );
}
