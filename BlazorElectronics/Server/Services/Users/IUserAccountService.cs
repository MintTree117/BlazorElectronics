using BlazorElectronics.Server.Models.Users;
using BlazorElectronics.Shared.Inbound.Users;

namespace BlazorElectronics.Server.Services.Users;

public interface IUserAccountService
{
    Task<Reply<UserLogin?>> ValidateUserLogin( string email, string password );
    Task<Reply<UserLogin?>> RegisterUser( UserRegisterRequest request );
    Task<Reply<int>> ValidateUserId( string username );
    Task<Reply<bool>> ChangePassword( int userId, string newPassword );
}