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

    public async Task<ServiceReply<List<int>?>> GetIds()
    {
        try
        {
            IEnumerable<int>? reply = await _userRepository.GetAllIds();

            return reply is not null
                ? new ServiceReply<List<int>?>( reply.ToList() )
                : new ServiceReply<List<int>?>( ServiceErrorType.NotFound );
        }
        catch ( Exception e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<List<int>?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<UserLoginDto?>> Login( string emailOrUsername, string password )
    {
        User? user;

        try
        {
            user = await _userRepository.GetByEmailOrUsername( emailOrUsername );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<UserLoginDto?>( ServiceErrorType.ServerError );
        }

        if ( user is null )
            return new ServiceReply<UserLoginDto?>( ServiceErrorType.NotFound, "User Not Found!" );

        return VerifyPasswordHash( password, user.PasswordHash, user.PasswordSalt )
            ? new ServiceReply<UserLoginDto?>( new UserLoginDto( user.UserId, user.Username, user.Email, user.IsAdmin ) )
            : new ServiceReply<UserLoginDto?>( ServiceErrorType.ValidationError, BAD_PASSWORD_MESSAGE );
    }
    public async Task<ServiceReply<UserLoginDto?>> Register( string username, string email, string password, string? phone )
    {
        try
        {
            UserExists? userExists = await _userRepository.GetUserExists( username, email );

            if ( userExists is not null )
                return new ServiceReply<UserLoginDto?>( ServiceErrorType.NotFound, GetUserExistsMessage( userExists, username, email ) );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<UserLoginDto?>( ServiceErrorType.ServerError );
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
            return new ServiceReply<UserLoginDto?>( ServiceErrorType.ServerError );
        }

        return insertedUser is not null
            ? new ServiceReply<UserLoginDto?>( new UserLoginDto( insertedUser.UserId, insertedUser.Username, insertedUser.Email, insertedUser.IsAdmin ) )
            : new ServiceReply<UserLoginDto?>( ServiceErrorType.NotFound );
    }
    public async Task<ServiceReply<bool>> VerifyAdmin( int adminId )
    {
        User? admin;

        try
        {
            admin = await _userRepository.GetById( adminId );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }

        if ( admin is null )
            return new ServiceReply<bool>( ServiceErrorType.NotFound );

        return admin.IsAdmin
            ? new ServiceReply<bool>( true )
            : new ServiceReply<bool>( ServiceErrorType.Unauthorized, NOT_ADMIN_MESSAGE );
    }
    public async Task<ServiceReply<int>> VerifyAdminId( int adminId )
    {
        User? admin;

        try
        {
            admin = await _userRepository.GetById( adminId );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<int>( ServiceErrorType.ServerError );
        }

        if ( admin is null )
            return new ServiceReply<int>( ServiceErrorType.NotFound );

        return admin.IsAdmin 
            ? new ServiceReply<int>( admin.UserId ) 
            : new ServiceReply<int>( ServiceErrorType.Unauthorized, NOT_ADMIN_MESSAGE );
    }
    public async Task<ServiceReply<int>> ValidateUserId( string email )
    {
        User? user;

        try
        {
            user = await _userRepository.GetByEmail( email );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<int>( ServiceErrorType.ServerError );
        }

        return user is not null 
            ? new ServiceReply<int>( user.UserId ) 
            : new ServiceReply<int>( ServiceErrorType.NotFound );
    }
    public async Task<ServiceReply<bool>> ChangePassword( int userId, string newPassword )
    {
        User? user;

        try
        {
            user = await _userRepository.GetById( userId );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }

        if ( user is null )
            return new ServiceReply<bool>( ServiceErrorType.NotFound );

        CreatePasswordHash( newPassword, out byte[] hash, out byte[] salt );

        user.PasswordHash = hash;
        user.PasswordSalt = salt;

        try
        {
            bool success = await _userRepository.UpdatePassword( userId, hash, salt );
            return success
                ? new ServiceReply<bool>( true )
                : new ServiceReply<bool>( ServiceErrorType.ValidationError );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
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