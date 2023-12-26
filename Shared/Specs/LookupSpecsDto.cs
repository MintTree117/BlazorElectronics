namespace BlazorElectronics.Shared.Specs;

public sealed class LookupSpecsDto
{
    public LookupSpecsDto()
    {
        
    }
    
    public LookupSpecsDto( List<int> globalIds, Dictionary<int, List<int>> categories, Dictionary<int, LookupSpec> specs )
    {
        GlobalSpecIds = globalIds;
        SpecIdsByCategory = categories;
        SpecsById = specs;
    }

    public List<int> GlobalSpecIds { get; init; } = new();
    public Dictionary<int, List<int>> SpecIdsByCategory { get; init; } = new();
    public Dictionary<int, LookupSpec> SpecsById { get; init; } = new();

    public List<LookupSpec> GetGlobalResponse()
    {
        return ( from specId in GlobalSpecIds select SpecsById[ specId ] ).ToList();
    }
    public List<LookupSpec> GetCategoryResponse( int category )
    {
        return ( from specId in SpecIdsByCategory[ category ] select SpecsById[ specId ] ).ToList();
    }
}