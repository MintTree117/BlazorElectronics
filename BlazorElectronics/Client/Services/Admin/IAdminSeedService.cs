using BlazorElectronics.Shared;

namespace BlazorElectronics.Client.Services.Admin;

public interface IAdminSeedService
{
    Task<ServiceReply<bool>> SeedProducts( IntDto amount );
    Task<ServiceReply<bool>> SeedUsers( IntDto amount );
}