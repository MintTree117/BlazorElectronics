namespace BlazorElectronics.Shared.SpecLookups;

public sealed class SpecLookupsResponse
{
    public SpecLookupsResponse( List<int> globalIds, Dictionary<int, List<int>> categories, Dictionary<int, SpecLookup> responses )
    {
        GlobalSpecIds = globalIds;
        SpecIdsByCategory = categories;
        ResponsesBySpecId = responses;
    }
    
    public List<int> GlobalSpecIds { get; init; }
    public Dictionary<int, List<int>> SpecIdsByCategory { get; init; }
    public Dictionary<int, SpecLookup> ResponsesBySpecId { get; init; }

    public List<SpecLookup> GetGlobalResponse()
    {
        return ( from specId in GlobalSpecIds select ResponsesBySpecId[ specId ] ).ToList();
    }
    public List<SpecLookup> GetCategoryResponse( int category )
    {
        return ( from specId in SpecIdsByCategory[ category ] select ResponsesBySpecId[ specId ] ).ToList();
    }
}