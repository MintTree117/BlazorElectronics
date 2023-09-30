namespace BlazorElectronics.Server.Models.Specs;

public sealed class CachedSpecLookups
{
    public Dictionary<int, List<object>> SpecLookupsBySpecId = new();

    public bool MissingData()
    {
        return SpecLookupsBySpecId == null;
    }
}