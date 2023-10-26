using BlazorElectronics.Server.Models.Users;
using BlazorElectronics.Shared.Inbound.Users;

namespace BlazorElectronics.Server.Services.Users;

public interface IUserAccountService
{
    Task<ServiceResponse<UserLogin?>> ValidateUserLogin( string email, string password );
    Task<ServiceResponse<UserLogin?>> RegisterUser( UserRegisterRequest request );
    Task<ServiceResponse<int>> ValidateUserId( string username );
    Task<ServiceResponse<bool>> ChangePassword( int userId, string newPassword );
}