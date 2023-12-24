namespace BlazorElectronics.Server.Api.Interfaces;

public interface IUserSeedService
{
    Task<ServiceReply<bool>> SeedUsers( int amount );
}