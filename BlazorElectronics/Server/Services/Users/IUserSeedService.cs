namespace BlazorElectronics.Server.Services.Users;

public interface IUserSeedService
{
    Task<ServiceReply<bool>> SeedUsers( int amount );
}