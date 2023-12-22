using BlazorElectronics.Shared;


namespace BlazorElectronics.Client.Services.Admin;

public interface IAdminSeedService
{
    Task<ServiceReply<bool>> SeedProducts( int amount );
    Task<ServiceReply<bool>> SeedReviews( int amount );
    Task<ServiceReply<bool>> SeedUsers( int amount );
}