using BlazorElectronics.Server.Models.Specs;

namespace BlazorElectronics.Server.Repositories.Specs;

public interface ISpecRepository
{
    Task<IEnumerable<Spec>?> GetSpecs();
    Task<IEnumerable<SpecLookup>?> GetSpecLookups();
}