using BlazorElectronics.Shared.DataTransferObjects.Specs;

namespace BlazorElectronics.Server.Services.Specs;

public interface ISpecService
{
    Task<ServiceResponse<Specs_DTO?>> GetSpecsDTO();
    Task<ServiceResponse<SpecLookups_DTO?>> GetSpecLookupsDTO();
}