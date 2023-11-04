using BlazorElectronics.Server.Models.Features;

namespace BlazorElectronics.Server.Repositories.Features;

public interface IFeatureRepository
{
    Task<IEnumerable<Feature>?> GetFeatures();
}