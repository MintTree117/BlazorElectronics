using BlazorElectronics.Server.Models.Specs;
using BlazorElectronics.Shared.DataTransferObjects.Specs;

namespace BlazorElectronics.Server.Services.Specs;

public interface ISpecCache
{
    Task<Specs_DTO?> GetSpecs();
    Task<SpecLookups_DTO?> GetSpecLookups();

    Task CacheSpecs( Specs_DTO dto );
    Task CacheSpecLookups( SpecLookups_DTO dto );
}