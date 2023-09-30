using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DataTransferObjects.Specs;

namespace BlazorElectronics.Server.Services.Specs;

public interface ISpecService
{
    Task<DtoResponse<SpecFilters_DTO>> GetSpecFilters( string categoryUrl );
    // GetSpecsByIds(List<int> specIds);
    // GetAllSpecData();
}