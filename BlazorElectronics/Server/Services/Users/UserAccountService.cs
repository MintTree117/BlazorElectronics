using System.Security.Cryptography;
using System.Text;
using BlazorElectronics.Server.Models.Users;
using BlazorElectronics.Server.Repositories.Users;
using BlazorElectronics.Shared.Inbound.Users;

namespace BlazorElectronics.Server.Services.Users;

public class UserAccountService : IUserAccountService
{
    readonly IUserRepository _userRepository;

    public UserAccountService( IUserRepository userRepository )
    {
        _userRepository = userRepository;
    }

    public async Task<Reply<UserLogin?>> ValidateUserLogin( string email, string password )
    {
        User? user;

        try
        {
            user = await _userRepository.GetByEmail( email );
        }
        catch ( ServiceException e )
        {
            return new Reply<UserLogin?>( e.Message );
        }
        
        return !VerifyPasswordHash( password, user!.PasswordHash, user.PasswordSalt ) 
            ? new Reply<UserLogin?>( null, false, "Incorrect password!" ) 
            : new Reply<UserLogin?>( new UserLogin( user.Id, user.Username ), true, $"Successfully validated user {email}." );
    }
    public async Task<Reply<UserLogin?>> RegisterUser( UserRegisterRequest request )
    {
        UserExists? userExists;

        try
        {
            userExists = await _userRepository.CheckIfUserExists( request.Username, request.Email );
            if ( userExists != null )
                return new Reply<UserLogin?>( GetUserExistsMessage( userExists, request ) );
        }
        catch ( ServiceException e )
        {
            return new Reply<UserLogin?>( e.Message );
        }

        CreatePasswordHash( request.Password, out byte[] hash, out byte[] salt );

        var newUserModel = new User {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = hash,
            PasswordSalt = salt,
            DateCreated = DateTime.Now
        };

        User? insertedUser = null;

        try
        {
            await _userRepository.AddUser( newUserModel );
        }
        catch ( ServiceException e )
        {
            return new Reply<UserLogin?>( e.Message );
        }

        return new Reply<UserLogin?>( new UserLogin( insertedUser!.Id, insertedUser.Username ), true, $"Successfully registered user {insertedUser.Username}" );
    }
    public async Task<Reply<int>> ValidateUserId( string username )
    {
        User? user = null;

        try
        {
            user = await _userRepository.GetByUsername( username );
        }
        catch ( ServiceException e )
        {
            return new Reply<int>( e.Message );
        }

        return new Reply<int>( user!.Id, false, $"Validated user {username}" );
    }
    public async Task<Reply<bool>> ChangePassword( int userId, string newPassword )
    {
        User? user = null;

        try
        {
            user = await _userRepository.GetById( userId );
        }
        catch ( ServiceException e )
        {
            return new Reply<bool>( e.Message );
        }

        CreatePasswordHash( newPassword, out byte[] hash, out byte[] salt );

        user!.PasswordHash = hash;
        user.PasswordSalt = salt;

        try
        {
            await _userRepository.UpdateUserPassword( userId, hash, salt );
        }
        catch ( ServiceException e )
        {
            return new Reply<bool>( e.Message );
        }

        return new Reply<bool>( true, true, $"Successfully updated password for user {user.Username}." );
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
    static string GetUserExistsMessage( UserExists existsObj, UserRegisterRequest request )
    {
        var messageBuilder = new StringBuilder();
        messageBuilder.Append( "User already exists with " );
        
        if ( existsObj.EmailExists )
        {
            messageBuilder.Append( $"email ({request.Email})" );

            if ( existsObj.UsernameExists )
            {
                messageBuilder.Append( $" and username ({request.Username})!" );
                return messageBuilder.ToString();
            }

            messageBuilder.Append( "!" );
            return messageBuilder.ToString();
        }

        messageBuilder.Append( $"username ({request.Username})!" );
        return messageBuilder.ToString();
    }
}