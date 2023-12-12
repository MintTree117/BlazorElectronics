using BlazorElectronics.Server.Core.Models.SpecLookups;
using BlazorElectronics.Shared.Specs;

namespace BlazorElectronics.Server.Core.Interfaces;

public interface ISpecRepository
{
    Task<SpecsModel?> Get();
    Task<IEnumerable<SpecModel>?> GetView();
    Task<SpecEditModel?> GetEdit( int specId );
    Task<int> Insert( SpecEdit dto );
    Task<bool> Update( SpecEdit dto );
    Task<bool> Delete( int specId );
}