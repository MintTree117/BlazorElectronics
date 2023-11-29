using BlazorElectronics.Server.Dtos.Users;
using BlazorElectronics.Shared.Users;

namespace BlazorElectronics.Server.Services.Users;

public sealed class UserSeedService : ApiService, IUserSeedService
{
    readonly IUserAccountService _userAccountService;
    
    public UserSeedService( ILogger<ApiService> logger, IUserAccountService userAccountService )
        : base( logger )
    {
        _userAccountService = userAccountService;
    }
    
    public async Task<ApiReply<bool>> SeedUsers( int amount )
    {
        List<UserRegisterRequest> seeds = await GetUserSeeds( amount );

        foreach ( UserRegisterRequest seed in seeds )
        {
            ApiReply<UserLoginDto?> seedReply = await _userAccountService.Register( seed.Username, seed.Username, seed.Password, seed.Phone );
            
            if ( !seedReply.Success )
                return new ApiReply<bool>( ServiceErrorType.ServerError, seedReply.Message );
        }

        return new ApiReply<bool>( true );
    }
    static async Task<List<UserRegisterRequest>> GetUserSeeds( int amount )
    {
        return await Task.Run( () =>
        {
            List<UserRegisterRequest> seeds = new();
            var generator = new UserAccountGenerator();

            for ( int i = 0; i < amount; i++ )
            {
                seeds.Add( generator.GenerateRandomUserAccount() );
            }

            return seeds;
        } );
    }

    class UserAccountGenerator
    {
        readonly HashSet<string> usedUsernames = new();
        readonly HashSet<string> usedEmails = new();
        readonly Random random = new();

        public UserRegisterRequest GenerateRandomUserAccount()
        {
            var userAccount = new UserRegisterRequest
            {
                Username = GenerateUniqueUsername(),
                Email = GenerateUniqueEmail(),
                Phone = GeneratePhoneNumber(),
                Password = GeneratePassword()
            };

            return userAccount;
        }

        string GenerateUniqueUsername()
        {
            string username;
            do
            {
                username = "User" + random.Next( 1000, 9999 );
            } while ( usedUsernames.Contains( username ) );

            usedUsernames.Add( username );
            return username;
        }
        string GenerateUniqueEmail()
        {
            string email;
            do
            {
                email = "user" + random.Next( 1000, 9999 ) + "@example.com";
            } while ( usedEmails.Contains( email ) );

            usedEmails.Add( email );
            return email;
        }
        string GeneratePhoneNumber()
        {
            return "555-" + random.Next( 100, 999 ) + "-" + random.Next( 1000, 9999 );
        }
        string GeneratePassword()
        {
            // This is a very basic password generator. Replace with a more robust algorithm as needed.
            return "Pass" + random.Next( 1000, 9999 ) + "!";
        }
    }
}