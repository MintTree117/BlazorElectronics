using BlazorElectronics.Server.Models.SpecLookups;
using BlazorElectronics.Shared.SpecLookups;

namespace BlazorElectronics.Server.Repositories.Specs;

public interface ISpecRepository
{
    Task<SpecsModel?> Get();
    Task<IEnumerable<SpecModel>?> GetView();
    Task<SpecEditModel?> GetEdit( int specId );
    Task<int> Insert( SpecEdit dto );
    Task<bool> Update( SpecEdit dto );
    Task<bool> Delete( int specId );
}