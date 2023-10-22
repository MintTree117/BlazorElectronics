using BlazorElectronics.Server.Models.Users;

namespace BlazorElectronics.Server.Repositories.Users;

public interface IUserRepository : IDapperRepository<User>
{
    Task<User?> GetByUsername( string username );
    Task<User?> GetByEmail( string email );
    Task<UserExists> CheckIfUserExists( string username, string email );
}