using BlazorElectronics.Server.Core.Models.SpecLookups;
using BlazorElectronics.Shared.Specs;

namespace BlazorElectronics.Server.Core.Interfaces;

public interface ISpecRepository : IDapperRepository
{
    Task<SpecsModel?> Get();
    Task<IEnumerable<SpecModel>?> GetView();
    Task<SpecEditModel?> GetEdit( int specId );
    Task<int> Insert( LookupSpecEditDto dto );
    Task<bool> Update( LookupSpecEditDto dto );
    Task<bool> Delete( int specId );
}