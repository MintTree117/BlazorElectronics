using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using BlazorElectronics.Server.Models.Users;
using BlazorElectronics.Server.Repositories.Users;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Inbound.Users;
using BlazorElectronics.Shared.Outbound.Users;
using Microsoft.IdentityModel.Tokens;

namespace BlazorElectronics.Server.Services.Users;

public class UserService : IUserService
{
    readonly IUserRepository _userRepository;
    readonly IConfiguration _configuration;
    
    public UserService( IUserRepository userRepository, IConfiguration configuration )
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<ServiceResponse<int>> RegisterUser( UserRegister_DTO newUser )
    {
        UserExists userExists = await _userRepository.CheckIfUserExists( newUser.Username, newUser.Email );

        if ( userExists.UsernameExists || userExists.EmailExists )
        {
            string message = "User already exists with";

            if ( userExists.UsernameExists )
            {
                message += $" username {newUser.Username}";
                if ( userExists.EmailExists )
                    message += $" and email {newUser.Email}";
            }
            else
            {
                message += $" with email {newUser.Email}";
            }

            return new ServiceResponse<int>( -1, false, message );
        }

        CreatePasswordHash( newUser.Password, out byte[] hash, out byte[] salt );

        var newUserModel = new User {
            Username = newUser.Username,
            Email = newUser.Email,
            PasswordHash = hash,
            PasswordSalt = salt,
            DateCreated = DateTime.Now
        };

        await _userRepository.Insert( newUserModel );
        User? insertedUser = await _userRepository.GetByUsername( newUserModel.Username );

        return insertedUser == null
            ? new ServiceResponse<int>( -1, false, "Failed to insert user into database!" )
            : new ServiceResponse<int>( insertedUser.Id, true, $"Successfully inserted user {insertedUser.Username} into database." );
    }
    public async Task<ServiceResponse<UserLoginResponse_DTO?>> LogUserIn( string email, string password )
    {
        User? user = await _userRepository.GetByEmail( email );

        if ( user == null )
            return new ServiceResponse<UserLoginResponse_DTO?>( null, false, $"User with email {email} does not exist!" );

        if ( !VerifyPasswordHash( password, user.PasswordHash, user.PasswordSalt ) )
            return new ServiceResponse<UserLoginResponse_DTO?>( null, false, "Incorrect password!" );

        return new ServiceResponse<UserLoginResponse_DTO?> {
            Data = new UserLoginResponse_DTO {
                Username = user.Username,
                JsonToken = CreateLoginToken( user )
            },
            Success = true,
            Message = $"Successfully validated and logged in user {user.Username}"
        };
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
    string CreateLoginToken( User user )
    {
        var claims = new List<Claim> {
            new( ClaimTypes.NameIdentifier, user.Id.ToString() ),
            new( ClaimTypes.Name, user.Email ),
        };

        var key = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection( "AppSettings:Token" ).Value! ) );

        var credentials = new SigningCredentials( key, SecurityAlgorithms.HmacSha512 );

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays( 1 ),
            signingCredentials: credentials );

        var jwt = new JwtSecurityTokenHandler().WriteToken( token );

        return jwt;
    }
}