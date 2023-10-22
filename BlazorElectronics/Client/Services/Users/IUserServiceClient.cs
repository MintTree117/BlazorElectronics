using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Inbound.Users;
using BlazorElectronics.Shared.Outbound.Users;

namespace BlazorElectronics.Client.Services.Users;

public interface IUserServiceClient
{
    Task<ServiceResponse<int>> Register( UserRegister_DTO request );
    Task<ServiceResponse<UserLoginResponse_DTO?>> Login( UserLoginRequest_DTO request );
}