using System.Security.Cryptography;
using System.Text;
using BlazorElectronics.Server.Dtos.Users;
using BlazorElectronics.Server.Models.Users;
using BlazorElectronics.Server.Repositories.Users;

namespace BlazorElectronics.Server.Services.Users;

public class UserAccountService : ApiService, IUserAccountService
{
    const string BAD_PASSWORD_MESSAGE = "Incorrect Password!";
    const string NOT_ADMIN_MESSAGE = "This account is not an administrator!";
    
    readonly IUserRepository _userRepository;

    public UserAccountService( ILogger<ApiService> logger, IUserRepository userRepository ) : base( logger )
    {
        _userRepository = userRepository;
    }

    public async Task<ApiReply<List<int>?>> GetIds()
    {
        try
        {
            List<int>? reply = await _userRepository.GetAllIds();

            return reply is not null
                ? new ApiReply<List<int>?>( reply )
                : new ApiReply<List<int>?>( ServiceErrorType.NotFound );
        }
        catch ( Exception e )
        {
            Logger.LogError( e.Message, e );
            return new ApiReply<List<int>?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ApiReply<UserLoginDto?>> Login( string emailOrUsername, string password )
    {
        User? user;

        try
        {
            user = await _userRepository.GetByEmailOrUsername( emailOrUsername );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ApiReply<UserLoginDto?>( ServiceErrorType.ServerError );
        }

        if ( user is null )
            return new ApiReply<UserLoginDto?>( ServiceErrorType.NotFound, "User Not Found!" );

        return VerifyPasswordHash( password, user.PasswordHash, user.PasswordSalt )
            ? new ApiReply<UserLoginDto?>( new UserLoginDto( user.UserId, user.Username, user.Email, user.IsAdmin ) )
            : new ApiReply<UserLoginDto?>( ServiceErrorType.ValidationError, BAD_PASSWORD_MESSAGE );
    }
    public async Task<ApiReply<UserLoginDto?>> Register( string username, string email, string password, string? phone )
    {
        try
        {
            UserExists? userExists = await _userRepository.GetUserExists( username, email );

            if ( userExists is not null )
                return new ApiReply<UserLoginDto?>( ServiceErrorType.NotFound, GetUserExistsMessage( userExists, username, email ) );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ApiReply<UserLoginDto?>( ServiceErrorType.ServerError );
        }

        CreatePasswordHash( password, out byte[] hash, out byte[] salt );
        User? insertedUser;

        try
        {
            insertedUser = await _userRepository.InsertUser( username, email, phone, hash, salt );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ApiReply<UserLoginDto?>( ServiceErrorType.ServerError );
        }

        return insertedUser is not null
            ? new ApiReply<UserLoginDto?>( new UserLoginDto( insertedUser.UserId, insertedUser.Username, insertedUser.Email, insertedUser.IsAdmin ) )
            : new ApiReply<UserLoginDto?>( ServiceErrorType.NotFound );
    }
    public async Task<ApiReply<int>> VerifyAdminId( int adminId )
    {
        User? admin;

        try
        {
            admin = await _userRepository.GetById( adminId );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ApiReply<int>( ServiceErrorType.ServerError );
        }

        if ( admin is null )
            return new ApiReply<int>( ServiceErrorType.NotFound );

        return admin.IsAdmin 
            ? new ApiReply<int>( admin.UserId ) 
            : new ApiReply<int>( ServiceErrorType.Unauthorized, NOT_ADMIN_MESSAGE );
    }
    public async Task<ApiReply<int>> ValidateUserId( string email )
    {
        User? user;

        try
        {
            user = await _userRepository.GetByEmail( email );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ApiReply<int>( ServiceErrorType.ServerError );
        }

        return user is not null 
            ? new ApiReply<int>( user.UserId ) 
            : new ApiReply<int>( ServiceErrorType.NotFound );
    }
    public async Task<ApiReply<bool>> ChangePassword( int userId, string newPassword )
    {
        User? user;

        try
        {
            user = await _userRepository.GetById( userId );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ApiReply<bool>( ServiceErrorType.ServerError );
        }

        if ( user is null )
            return new ApiReply<bool>( ServiceErrorType.NotFound );

        CreatePasswordHash( newPassword, out byte[] hash, out byte[] salt );

        user.PasswordHash = hash;
        user.PasswordSalt = salt;

        try
        {
            bool success = await _userRepository.UpdatePassword( userId, hash, salt );
            return success
                ? new ApiReply<bool>( true )
                : new ApiReply<bool>( ServiceErrorType.ValidationError );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ApiReply<bool>( ServiceErrorType.ServerError );
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