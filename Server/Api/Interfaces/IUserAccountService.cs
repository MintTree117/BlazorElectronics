using BlazorElectronics.Server.Core.Models.Users;
using BlazorElectronics.Shared.Users;

namespace BlazorElectronics.Server.Api.Interfaces;

public interface IUserAccountService
{
    Task<ServiceReply<List<int>?>> GetAllIds();
    Task<ServiceReply<AccountDetailsDto?>> GetAccountDetails( int userId );
    Task<ServiceReply<UserLoginDto?>> Login( string emailOrUsername, string password );
    Task<ServiceReply<bool>> Register( string username, string email, string password, string? phone );
    Task<ServiceReply<bool>> ResendVerificationEmail( string token );
    Task<ServiceReply<AccountDetailsDto?>> UpdateAccountDetails( int userId, AccountDetailsDto dto );
    Task<ServiceReply<bool>> ChangePassword( int userId, string newPassword );
    Task<ServiceReply<bool>> ActivateAccount( string token );
}