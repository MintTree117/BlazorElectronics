using BlazorElectronics.Server.Models.Users;

namespace BlazorElectronics.Server.Repositories.Users;

public interface IUserRepository
{
    Task<User?> GetById( int id );
    Task<User?> GetByUsername( string username );
    Task<User?> GetByEmail( string email );
    Task<User?> GetByEmailOrUsername( string emailOrUsername );
    Task<UserExists?> GetUserExists( string username, string email );
    Task<User?> InsertUser( string username, string email, string? phone, byte[] hash, byte[] salt );
    Task<bool> UpdatePassword( int id, byte[] hash, byte[] salt );
}
