using System.Security.Cryptography;
using System.Text;
using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Server.Core.Models.Users;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.Users;

namespace BlazorElectronics.Server.Core.Services;

public sealed class UserAccountService : _ApiService, IUserAccountService
{
    const string BAD_PASSWORD_MESSAGE = "Incorrect Password!";

    readonly IUserAccountRepository _userAccountRepository;
    readonly IEmailService _emailService;

    public UserAccountService( ILogger<_ApiService> logger, IUserAccountRepository userAccountRepository, IEmailService emailService ) : base( logger )
    {
        _userAccountRepository = userAccountRepository;
        _emailService = emailService;
    }

    public async Task<ServiceReply<List<int>?>> GetAllIds()
    {
        try
        {
            IEnumerable<int>? reply = await _userAccountRepository.GetAllIds();

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
    public async Task<ServiceReply<AccountDetailsDto?>> GetAccountDetails( int userId )
    {
        try
        {
            AccountDetailsDto? details = await _userAccountRepository.GetAccountDetails( userId );

            return details is not null
                ? new ServiceReply<AccountDetailsDto?>( details )
                : new ServiceReply<AccountDetailsDto?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<AccountDetailsDto?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<UserLoginDto?>> Login( string emailOrUsername, string password )
    {
        try
        {
            UserModel? user = await _userAccountRepository.GetByEmailOrUsername( emailOrUsername );

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
    public async Task<ServiceReply<bool>> Register( string username, string email, string password, string? phone )
    {
        try
        {
            UserExists? userExists = await _userAccountRepository.GetUserExists( username, email );
            
            if ( userExists is not null && ( userExists.EmailExists is not null || userExists.UsernameExists is not null ) )
                return new ServiceReply<bool>( ServiceErrorType.NotFound, GetUserExistsMessage( userExists, username, email ) );

            CreatePasswordHash( password, out byte[] hash, out byte[] salt );
            UserModel? insertedUser = await _userAccountRepository.InsertUser( username, email, phone, hash, salt );

            if ( insertedUser is null )
                return new ServiceReply<bool>( ServiceErrorType.ServerError, "Failed to insert user!" );

            string? token = await TryInsertVerificationToken( insertedUser.UserId, email );

            if ( token is null )
                return new ServiceReply<bool>( ServiceErrorType.ServerError, "Failed to insert verification token!" );

            SendVerificationEmail( email, token );

            return new ServiceReply<bool>( true );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<bool>> ResendVerificationEmail( string email )
    {
        try
        {
            string? token = await _userAccountRepository.GetVerificationToken( email );

            if ( string.IsNullOrWhiteSpace( token ) )
                return new ServiceReply<bool>( ServiceErrorType.NotFound );

            SendVerificationEmail( email, token );

            return new ServiceReply<bool>( true );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
    }

    public async Task<ServiceReply<AccountDetailsDto?>> UpdateAccountDetails( int userId, AccountDetailsDto dto )
    {
        try
        {
            AccountDetailsDto? details = await _userAccountRepository.UpdateAccountDetails( userId, dto );

            return details is not null
                ? new ServiceReply<AccountDetailsDto?>( details )
                : new ServiceReply<AccountDetailsDto?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<AccountDetailsDto?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<bool>> ChangePassword( int userId, string newPassword )
    {
        try
        {
            UserModel? user = await _userAccountRepository.GetById( userId );

            if ( user is null )
                return new ServiceReply<bool>( ServiceErrorType.NotFound );

            if ( !user.IsActive )
                return new ServiceReply<bool>( ServiceErrorType.Unauthorized, "You account has not yet been activated." );

            CreatePasswordHash( newPassword, out byte[] hash, out byte[] salt );

            bool success = await _userAccountRepository.UpdatePassword( userId, hash, salt );
            
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
            int userId = await _userAccountRepository.Update_VerificationToken( token );

            if ( userId <= 0 )
                return new ServiceReply<bool>( ServiceErrorType.ServerError, "Failed to activate account!" );

            bool activated = await _userAccountRepository.Update_UserAccountStatus( userId );

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

    async Task<string?> TryInsertVerificationToken( int userId, string userEmail )
    {
        const int safety = 1000;
        int count = 0;
        string token = string.Empty;
        bool tokenInserted = false;
        
        while ( count < safety )
        {
            token = Guid.NewGuid().ToString();

            if ( await _userAccountRepository.InsertVerificationToken( userId, userEmail, token ) )
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
    void SendVerificationEmail( string email, string token )
    {
        string subject = "Email Verification";
        string verificationUrl = $"https://blazormedia.azurewebsites.net/verify?token={token}";
        string body = $"Please verify your email by clicking on this link: {verificationUrl}";

        _emailService.SendEmailAsync( email, subject, body );
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
        
        if ( existsObj.EmailExists is not null )
        {
            messageBuilder.Append( $"email ({email})" );

            if ( existsObj.UsernameExists is not null )
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