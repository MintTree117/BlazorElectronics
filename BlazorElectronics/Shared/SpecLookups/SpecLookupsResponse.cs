namespace BlazorElectronics.Shared.SpecLookups;

public sealed class SpecLookupsResponse
{
    public SpecLookupsResponse( List<int> globalIds, Dictionary<int, List<int>> categories, Dictionary<int, SpecLookupDto> responses )
    {
        GlobalSpecIds = globalIds;
        SpecIdsByCategory = categories;
        ResponsesBySpecId = responses;
    }

    public List<int> GlobalSpecIds { get; init; }
    public Dictionary<int, List<int>> SpecIdsByCategory { get; init; }
    public Dictionary<int, SpecLookupDto> ResponsesBySpecId { get; init; }

    public List<SpecLookupDto> GetGlobalResponse()
    {
        return ( from specId in GlobalSpecIds select ResponsesBySpecId[ specId ] ).ToList();
    }
    public List<SpecLookupDto> GetCategoryResponse( int category )
    {
        return ( from specId in SpecIdsByCategory[ category ] select ResponsesBySpecId[ specId ] ).ToList();
    }
}