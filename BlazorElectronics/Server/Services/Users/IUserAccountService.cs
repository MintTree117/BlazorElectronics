using BlazorElectronics.Server.Dtos.Users;

namespace BlazorElectronics.Server.Services.Users;

public interface IUserAccountService
{
    Task<ApiReply<UserLoginDto?>> Login( string emailOrUsername, string password );
    Task<ApiReply<UserLoginDto?>> Register( string username, string email, string password, int? phone );
    Task<ApiReply<int>> VerifyAdminId( string email );
    Task<ApiReply<int>> ValidateUserId( string email );
    Task<ApiReply<bool>> ChangePassword( int userId, string newPassword );
}