using System.Collections.Concurrent;
using BlazorElectronics.Server.Models.Specs;

namespace BlazorElectronics.Server.Services.Specs;

public interface ISpecService
{
    Task<ServiceResponse<SpecMetaData>> GetSpecMetaData();
    Task<ServiceResponse<Dictionary<int, List<object>>>> GetSpecLookups();

    // Client Will Request...
    // - all lookup specs with their values, both for all, and per category, or multiple categories
    // - all raw specs... but not their values
}