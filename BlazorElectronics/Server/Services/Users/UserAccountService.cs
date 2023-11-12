using System.Security.Cryptography;
using System.Text;
using BlazorElectronics.Server.Dtos.Users;
using BlazorElectronics.Server.Models.Users;
using BlazorElectronics.Server.Repositories.Users;

namespace BlazorElectronics.Server.Services.Users;

public class UserAccountService : ApiService, IUserAccountService
{
    const string BAD_PASSWORD_MESSAGE = "Incorrect Password!";
    
    readonly IUserRepository _userRepository;

    public UserAccountService( ILogger logger, IUserRepository userRepository ) : base( logger )
    {
        _userRepository = userRepository;
    }

    public async Task<ApiReply<UserLoginDto?>> Login( string emailOrUsername, string password )
    {
        User? user;

        try
        {
            user = await _userRepository.GetByEmailOrUsername( emailOrUsername );
            
            if ( user is null )
                return new ApiReply<UserLoginDto?>( NO_DATA_FOUND_MESSAGE );
        }
        catch ( ServiceException e )
        {
            _logger.LogError( e.Message, e );
            return new ApiReply<UserLoginDto?>( INTERNAL_SERVER_ERROR_MESSAGE );
        }

        return VerifyPasswordHash( password, user.PasswordHash, user.PasswordSalt )
            ? new ApiReply<UserLoginDto?>( new UserLoginDto( user.Id, user.Email, user.Username, user.IsAdmin ) )
            : new ApiReply<UserLoginDto?>( BAD_PASSWORD_MESSAGE );
    }
    public async Task<ApiReply<UserLoginDto?>> Register( string username, string email, string password, int? phone )
    {
        try
        {
            UserExists? userExists = await _userRepository.GetUserExists( username, email );

            if ( userExists is not null )
                return new ApiReply<UserLoginDto?>( GetUserExistsMessage( userExists, username, email ) );
        }
        catch ( ServiceException e )
        {
            _logger.LogError( e.Message, e );
            return new ApiReply<UserLoginDto?>( INTERNAL_SERVER_ERROR_MESSAGE );
        }

        CreatePasswordHash( password, out byte[] hash, out byte[] salt );
        User? insertedUser;

        try
        {
            insertedUser = await _userRepository.AddUser( username, email, phone, hash, salt );

            if ( insertedUser is null )
                return new ApiReply<UserLoginDto?>( NO_DATA_FOUND_MESSAGE );
        }
        catch ( ServiceException e )
        {
            _logger.LogError( e.Message, e );
            return new ApiReply<UserLoginDto?>( INTERNAL_SERVER_ERROR_MESSAGE );
        }

        return new ApiReply<UserLoginDto?>( 
            new UserLoginDto( insertedUser.Id, insertedUser.Username, insertedUser.Email, insertedUser.IsAdmin ) );
    }
    public async Task<ApiReply<int>> ValidateUserId( string email )
    {
        User? user;

        try
        {
            user = await _userRepository.GetByEmail( email );

            if ( user is null )
                return new ApiReply<int>( NO_DATA_FOUND_MESSAGE );
        }
        catch ( ServiceException e )
        {
            _logger.LogError( e.Message, e );
            return new ApiReply<int>( INTERNAL_SERVER_ERROR_MESSAGE );
        }

        return new ApiReply<int>( user.Id );
    }
    public async Task<ApiReply<bool>> ChangePassword( int userId, string newPassword )
    {
        User? user;

        try
        {
            user = await _userRepository.GetById( userId );

            if ( user is null )
                return new ApiReply<bool>( NO_DATA_FOUND_MESSAGE );
        }
        catch ( ServiceException e )
        {
            _logger.LogError( e.Message, e );
            return new ApiReply<bool>( INTERNAL_SERVER_ERROR_MESSAGE );
        }

        CreatePasswordHash( newPassword, out byte[] hash, out byte[] salt );

        user.PasswordHash = hash;
        user.PasswordSalt = salt;

        try
        {
            bool success = await _userRepository.UpdatePassword( userId, hash, salt );

            return new ApiReply<bool>( success );
        }
        catch ( ServiceException e )
        {
            _logger.LogError( e.Message, e );
            return new ApiReply<bool>( INTERNAL_SERVER_ERROR_MESSAGE );
        }
    }

    static void CreatePasswordHash( string password, out byte[] hash, out byte[] salt )
    {
        var hmac = new HMACSHA512();
        byte[] passwordBytes = Encoding.UTF8.GetBytes( password );

        salt = hmac.Key;
        hash = hmac.ComputeHash( passwordBytes );

        hmac.Dispose();
    }
    static bool VerifyPasswordHash( string password, byte[] hash, byte[] salt )
    {
        var hmac = new HMACSHA512( salt );
        byte[] computedHash = hmac.ComputeHash( Encoding.UTF8.GetBytes( password ) );
        return computedHash.SequenceEqual( hash );
    }
    static string GetUserExistsMessage( UserExists existsObj, string username, string email )
    {
        var messageBuilder = new StringBuilder();
        messageBuilder.Append( "User already exists with " );
        
        if ( existsObj.EmailExists )
        {
            messageBuilder.Append( $"email ({email})" );

            if ( existsObj.UsernameExists )
            {
                messageBuilder.Append( $" and username ({username})!" );
                return messageBuilder.ToString();
            }

            messageBuilder.Append( "!" );
            return messageBuilder.ToString();
        }

        messageBuilder.Append( $"username ({username})!" );
        return messageBuilder.ToString();
    }
}