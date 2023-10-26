using System.Security.Cryptography;
using BlazorElectronics.Server.Models.Users;
using BlazorElectronics.Server.Repositories.Users;
using BlazorElectronics.Shared.Inbound.Users;
using BlazorElectronics.Shared.Mutual;

namespace BlazorElectronics.Server.Services.Users;

public class UserAccountService : IUserAccountService
{
    readonly IUserRepository _userRepository;

    public UserAccountService( IUserRepository userRepository, IConfiguration configuration )
    {
        _userRepository = userRepository;
    }

    public async Task<ServiceResponse<UserLogin?>> ValidateUserLogin( string email, string password )
    {
        User? user = await _userRepository.GetByEmail( email );
        
        if ( user == null )
            return new ServiceResponse<UserLogin?>( null, false, $"User {email} does not exist!" );

        return !VerifyPasswordHash( password, user.PasswordHash, user.PasswordSalt ) 
            ? new ServiceResponse<UserLogin?>( null, false, "Incorrect password!" ) 
            : new ServiceResponse<UserLogin?>( new UserLogin( user.Id, user.Username ), true, $"Successfully validated user {email}." );
    }
    public async Task<ServiceResponse<UserLogin?>> RegisterUser( UserRegisterRequest request )
    {
        UserExists? userExists = await _userRepository.CheckIfUserExists( request.Username, request.Email );

        if ( userExists != null )
            return new ServiceResponse<UserLogin?>( null, false, GetUserExistsMessage( request, userExists ) );

        CreatePasswordHash( request.Password, out byte[] hash, out byte[] salt );

        var newUserModel = new User {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = hash,
            PasswordSalt = salt,
            DateCreated = DateTime.Now
        };

        User? insertedUser = await _userRepository.AddUser( newUserModel );

        return insertedUser == null
            ? new ServiceResponse<UserLogin?>( null, false, $"Failed to insert user {request.Email} into database!" )
            : new ServiceResponse<UserLogin?>( new UserLogin( insertedUser.Id, insertedUser.Username ), true, $"Successfully registered user {insertedUser.Username}" );
    }
    public async Task<ServiceResponse<int>> ValidateUserId( string username )
    {
        User? user = await _userRepository.GetByUsername( username );

        return user == null
            ? new ServiceResponse<int>( -1, false, $"User {username} does not exist!" )
            : new ServiceResponse<int>( user.Id, false, $"Validated user {username}" );
    }
    public async Task<ServiceResponse<bool>> ChangePassword( int userId, string newPassword )
    {
        User? user = await _userRepository.GetById( userId );

        if ( user == null )
            return new ServiceResponse<bool>( false, false, "User not found!" );

        CreatePasswordHash( newPassword, out byte[] hash, out byte[] salt );

        user.PasswordHash = hash;
        user.PasswordSalt = salt;

        bool success = await _userRepository.UpdateUserPassword( userId, hash, salt );

        return success 
            ? new ServiceResponse<bool>( success, success, $"Successfully updated password for user {user.Username}" ) 
            : new ServiceResponse<bool>( success, success, $"Failed to update password for user {user.Username}" );
    }

    static void CreatePasswordHash( string password, out byte[] hash, out byte[] salt )
    {
        var hmac = new HMACSHA512();
        var passwordBytes = System.Text.Encoding.UTF8.GetBytes( password );

        salt = hmac.Key;
        hash = hmac.ComputeHash( passwordBytes );

        hmac.Dispose();
    }
    static bool VerifyPasswordHash( string password, byte[] hash, byte[] salt )
    {
        var hmac = new HMACSHA512( salt );
        byte[] computedHash = hmac.ComputeHash( System.Text.Encoding.UTF8.GetBytes( password ) );
        return computedHash.SequenceEqual( hash );
    }
    static string GetUserExistsMessage( UserRegisterRequest request, UserExists userExists )
    {
        string message = "User already exists with";

        if ( userExists.UsernameExists )
        {
            message += $" username {request.Username}";
            if ( userExists.EmailExists )
                message += $" and email {request.Email}";
        }
        else
        {
            message += $" with email {request.Email}";
        }

        return message;
    }
}