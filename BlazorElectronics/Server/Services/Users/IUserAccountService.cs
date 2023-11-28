using BlazorElectronics.Server.Dtos.Users;

namespace BlazorElectronics.Server.Services.Users;

public interface IUserAccountService
{
    Task<ApiReply<List<int>?>> GetIds();
    Task<ApiReply<UserLoginDto?>> Login( string emailOrUsername, string password );
    Task<ApiReply<UserLoginDto?>> Register( string username, string email, string password, string? phone );
    Task<ApiReply<int>> VerifyAdminId( int adminId );
    Task<ApiReply<int>> ValidateUserId( string email );
    Task<ApiReply<bool>> ChangePassword( int userId, string newPassword );
}