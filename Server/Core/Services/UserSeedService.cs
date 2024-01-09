using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Server.Core.Models.Users;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.Users;

namespace BlazorElectronics.Server.Core.Services;

public sealed class UserSeedService : _ApiService, IUserSeedService
{
    readonly IUserAccountService _userAccountService;
    
    public UserSeedService( ILogger<UserSeedService> logger, IUserAccountService userAccountService )
        : base( logger )
    {
        _userAccountService = userAccountService;
    }
    
    public async Task<ServiceReply<bool>> SeedUsers( int amount )
    {
        List<RegisterRequestDto> seeds = await GetUserSeeds( amount );

        foreach ( RegisterRequestDto seed in seeds )
        {
            ServiceReply<bool> seedReply = await _userAccountService.Register( seed.Username, seed.Username, seed.Password, seed.Phone );
            
            if ( !seedReply.Success )
                return new ServiceReply<bool>( ServiceErrorType.ServerError, seedReply.Message );
        }

        return new ServiceReply<bool>( true );
    }
    static async Task<List<RegisterRequestDto>> GetUserSeeds( int amount )
    {
        return await Task.Run( () =>
        {
            List<RegisterRequestDto> seeds = new();
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

        public RegisterRequestDto GenerateRandomUserAccount()
        {
            var userAccount = new RegisterRequestDto
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