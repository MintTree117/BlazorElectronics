using BlazorElectronics.Server.Dtos.Users;

namespace BlazorElectronics.Server.Services.Users;

public interface IUserAccountService
{
    Task<ServiceReply<List<int>?>> GetIds();
    Task<ServiceReply<UserLoginDto?>> Login( string emailOrUsername, string password );
    Task<ServiceReply<UserLoginDto?>> Register( string username, string email, string password, string? phone );
    Task<ServiceReply<bool>> VerifyAdmin( int adminId );
    Task<ServiceReply<int>> VerifyAdminId( int adminId );
    Task<ServiceReply<int>> ValidateUserId( string email );
    Task<ServiceReply<bool>> ChangePassword( int userId, string newPassword );
}