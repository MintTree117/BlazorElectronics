using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Inbound.Users;
using BlazorElectronics.Shared.Outbound.Users;

namespace BlazorElectronics.Server.Services.Users;

public interface IUserService
{
    Task<ServiceResponse<int>> RegisterUser( UserRegister_DTO newUser );
    Task<ServiceResponse<UserLoginResponse_DTO?>> LogUserIn( string email, string password );
}