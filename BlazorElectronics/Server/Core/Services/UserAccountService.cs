using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Server.Core.Models.Users;
using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Server.Core.Services;

public sealed class UserAccountService : ApiService, IUserAccountService
{
    const string BAD_PASSWORD_MESSAGE = "Incorrect Password!";
    const string NOT_ADMIN_MESSAGE = "This account is not an administrator!";
    
    readonly IUserRepository _userRepository;

    public UserAccountService( ILogger<ApiService> logger, IUserRepository userRepository ) : base( logger )
    {
        _userRepository = userRepository;
    }

    public async Task<ServiceReply<List<int>?>> GetAllIds()
    {
        try
        {
            IEnumerable<int>? reply = await _userRepository.GetAllIds();

            return reply is not null
                ? new ServiceReply<List<int>?>( reply.ToList() )
                : new ServiceReply<List<int>?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<List<int>?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<UserLoginDto?>> Login( string emailOrUsername, string password )
    {
        try
        {
            UserModel? user = await _userRepository.GetByEmailOrUsername( emailOrUsername );

            if ( user is null )
                return new ServiceReply<UserLoginDto?>( ServiceErrorType.NotFound, "User Not Found!" );

            if ( !user.IsActive )
                return new ServiceReply<UserLoginDto?>( ServiceErrorType.Unauthorized, "You account has not yet been activated." );

            return VerifyPasswordHash( password, user.PasswordHash, user.PasswordSalt )
                ? new ServiceReply<UserLoginDto?>( new UserLoginDto( user.UserId, user.Username, user.Email, user.IsAdmin ) )
                : new ServiceReply<UserLoginDto?>( ServiceErrorType.ValidationError, BAD_PASSWORD_MESSAGE );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<UserLoginDto?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<UserLoginDto?>> Register( string username, string email, string password, string? phone )
    {
        try
        {
            UserExists? userExists = await _userRepository.GetUserExists( username, email );

            if ( userExists is not null )
                return new ServiceReply<UserLoginDto?>( ServiceErrorType.NotFound, GetUserExistsMessage( userExists, username, email ) );

            CreatePasswordHash( password, out byte[] hash, out byte[] salt );
            UserModel? insertedUser = await _userRepository.InsertUser( username, email, phone, hash, salt );

            if ( insertedUser is null )
                return new ServiceReply<UserLoginDto?>( ServiceErrorType.ServerError, "Failed to insert user!" );

            string? token = await TryInsertVerificationToken( insertedUser.UserId );

            if ( token is null )
                return new ServiceReply<UserLoginDto?>( ServiceErrorType.ServerError, "Failed to insert verification token!" );

            SendVerificationEmail( email, token );

            return new ServiceReply<UserLoginDto?>( new UserLoginDto( insertedUser.UserId, insertedUser.Username, insertedUser.Email, insertedUser.IsAdmin ) );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<UserLoginDto?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<bool>> VerifyAdmin( int adminId )
    {
        try
        {
            UserModel? admin = await _userRepository.GetById( adminId );

            if ( admin is null )
                return new ServiceReply<bool>( ServiceErrorType.NotFound );

            if ( !admin.IsActive )
                return new ServiceReply<bool>( ServiceErrorType.Unauthorized, "You account has not yet been activated." );

            return admin.IsAdmin
                ? new ServiceReply<bool>( true )
                : new ServiceReply<bool>( ServiceErrorType.Unauthorized, NOT_ADMIN_MESSAGE );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<int>> VerifyAdminId( int adminId )
    {
        try
        {
            UserModel? admin = await _userRepository.GetById( adminId );

            if ( admin is null )
                return new ServiceReply<int>( ServiceErrorType.NotFound );

            if ( !admin.IsActive )
                return new ServiceReply<int>( ServiceErrorType.Unauthorized, "You account has not yet been activated." );

            return admin.IsAdmin
                ? new ServiceReply<int>( admin.UserId )
                : new ServiceReply<int>( ServiceErrorType.Unauthorized, NOT_ADMIN_MESSAGE );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<int>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<int>> ValidateUserId( int id )
    {
        try
        {
            UserModel? user = await _userRepository.GetById( id );

            if ( user is null )
                return new ServiceReply<int>( ServiceErrorType.NotFound );

            return user.IsActive
                ? new ServiceReply<int>( user.UserId )
                : new ServiceReply<int>( ServiceErrorType.Unauthorized, "You account has not yet been activated." );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<int>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<bool>> ChangePassword( int userId, string newPassword )
    {
        try
        {
            UserModel? user = await _userRepository.GetById( userId );

            if ( user is null )
                return new ServiceReply<bool>( ServiceErrorType.NotFound );

            if ( !user.IsActive )
                return new ServiceReply<bool>( ServiceErrorType.Unauthorized, "You account has not yet been activated." );

            CreatePasswordHash( newPassword, out byte[] hash, out byte[] salt );

            bool success = await _userRepository.UpdatePassword( userId, hash, salt );
            
            return success
                ? new ServiceReply<bool>( true )
                : new ServiceReply<bool>( ServiceErrorType.ValidationError );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<bool>> ActivateAccount( string token )
    {
        try
        {
            int userId = await _userRepository.Update_VerificationToken( token );

            if ( userId <= 0 )
                return new ServiceReply<bool>( ServiceErrorType.ServerError, "Failed to activate account!" );

            bool activated = await _userRepository.Update_UserAccountStatus( userId );

            return activated
                ? new ServiceReply<bool>( true )
                : new ServiceReply<bool>( ServiceErrorType.ServerError, "Failed to activate account!" );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError, "Failed to activate account!" );
        }
    }

    async Task<string?> TryInsertVerificationToken( int userId )
    {
        const int safety = 1000;
        int count = 0;
        string token = string.Empty;
        bool tokenInserted = false;
        
        while ( count < safety )
        {
            token = Guid.NewGuid().ToString();

            if ( await _userRepository.InsertVerificationCode( userId, token ) )
            {
                tokenInserted = true;
                break;   
            }

            count++;
        }

        return tokenInserted
            ? token
            : null;
    }
    static void SendVerificationEmail( string email, string token )
    {
        SmtpClient smtpClient = new( "smtp.gmail.com", 587 )
        {
            Credentials = new NetworkCredential( "martygrof3708@gmail.com", "yourPassword" ),
            EnableSsl = true
        };

        string verificationUrl = $"http://blazorelectronics.com/verify?token={token}";

        MailMessage mail = new()
        {
            From = new MailAddress( "martygrof3708@gmail.com" ),
            Subject = "Email Verification",
            Body = $"Please verify your email by clicking on this link: {verificationUrl}",
            IsBodyHtml = true
        };
        mail.To.Add( new MailAddress( email ) ); // userEmail is the recipient's email address
        smtpClient.SendAsync( mail, null );
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