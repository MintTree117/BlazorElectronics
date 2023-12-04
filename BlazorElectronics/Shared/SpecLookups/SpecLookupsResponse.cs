using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Shared.SpecLookups;

public sealed class SpecLookupsResponse
{
    public SpecLookupsResponse( List<int> globalIds, Dictionary<PrimaryCategory, List<int>> categories, Dictionary<int, SpecLookupDto> responses )
    {
        GlobalSpecIds = globalIds;
        SpecIdsByCategory = categories;
        ResponsesBySpecId = responses;
    }

    public List<int> GlobalSpecIds { get; init; }
    public Dictionary<PrimaryCategory, List<int>> SpecIdsByCategory { get; init; }
    public Dictionary<int, SpecLookupDto> ResponsesBySpecId { get; init; }

    public List<SpecLookupDto> GetGlobalResponse()
    {
        return ( from specId in GlobalSpecIds select ResponsesBySpecId[ specId ] ).ToList();
    }
    public List<SpecLookupDto> GetCategoryResponse( PrimaryCategory category )
    {
        return ( from specId in SpecIdsByCategory[ category ] select ResponsesBySpecId[ specId ] ).ToList();
    }
    public List<PrimaryCategory> GetSpecCategories( int specId )
    {
        PrimaryCategory[] categories = Enum.GetValues<PrimaryCategory>();

        return categories
            .Where( c => SpecIdsByCategory[ c ]
                .Contains( specId ) )
            .ToList();
    }
}