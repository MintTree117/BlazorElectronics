namespace BlazorElectronics.Shared.SpecLookups;

public sealed class SpecsResponse
{
    public SpecsResponse( List<int> globalIds, Dictionary<int, List<int>> categories, Dictionary<int, Spec> specs )
    {
        GlobalSpecIds = globalIds;
        SpecIdsByCategory = categories;
        SpecsById = specs;
    }
    
    public List<int> GlobalSpecIds { get; init; }
    public Dictionary<int, List<int>> SpecIdsByCategory { get; init; }
    public Dictionary<int, Spec> SpecsById { get; init; }

    public List<Spec> GetGlobalResponse()
    {
        return ( from specId in GlobalSpecIds select SpecsById[ specId ] ).ToList();
    }
    public List<Spec> GetCategoryResponse( int category )
    {
        return ( from specId in SpecIdsByCategory[ category ] select SpecsById[ specId ] ).ToList();
    }
}