using BlazorElectronics.Server.Models.Specs;

namespace BlazorElectronics.Server.Repositories.Specs;

public interface ISpecRepository
{
    Task<Dictionary<string, Spec>?> GetSpecs();
    Task<Dictionary<int, List<object>>?> GetSpecLookups();
}