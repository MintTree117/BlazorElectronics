using BlazorElectronics.Server.Core.Models.Users;
using BlazorElectronics.Shared.Users;

namespace BlazorElectronics.Server.Api.Interfaces;

public interface IUserAccountService
{
    Task<ServiceReply<List<int>?>> GetAllIds();
    Task<ServiceReply<AccountDetailsDto?>> GetAccountDetails( int userId );
    Task<ServiceReply<UserLoginDto?>> Login( string emailOrUsername, string password );
    Task<ServiceReply<UserLoginDto?>> Register( string username, string email, string password, string? phone );
    Task<ServiceReply<bool>> VerifyAdmin( int adminId );
    Task<ServiceReply<int>> VerifyAdminId( int adminId );
    Task<ServiceReply<int>> ValidateUserId( int id );
    Task<ServiceReply<AccountDetailsDto?>> UpdateAccountDetails( int userId, AccountDetailsDto dto );
    Task<ServiceReply<bool>> ChangePassword( int userId, string newPassword );
    Task<ServiceReply<bool>> ActivateAccount( string token );
}