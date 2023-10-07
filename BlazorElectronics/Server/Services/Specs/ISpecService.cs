using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DataTransferObjects.Specs;

namespace BlazorElectronics.Server.Services.Specs;

public interface ISpecService
{
    Task<ServiceResponse<SpecFilters_DTO>> GetSpecFilters( string categoryUrl );
    Task<ServiceResponse<SpecFilters_DTO>> GetSpecFilters( int categoryId );
    // GetSpecsByIds(List<int> specIds);
    // GetAllSpecData();
}