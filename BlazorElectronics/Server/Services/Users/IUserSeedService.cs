namespace BlazorElectronics.Server.Services.Users;

public interface IUserSeedService
{
    Task<ApiReply<bool>> SeedUsers( int amount );
}