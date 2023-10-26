using BlazorElectronics.Server.Models.Users;

namespace BlazorElectronics.Server.Repositories.Users;

public interface IUserRepository
{
    Task<User?> GetById( int id );
    Task<User?> GetByUsername( string username );
    Task<User?> GetByEmail( string email );
    Task<UserExists?> CheckIfUserExists( string username, string email );
    Task<bool> UpdateUserPassword( int id, byte[] hash, byte[] salt );
    Task<User?> AddUser( User user );
}
