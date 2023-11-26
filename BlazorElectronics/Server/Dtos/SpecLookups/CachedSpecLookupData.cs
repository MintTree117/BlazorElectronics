using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.SpecLookups;

namespace BlazorElectronics.Server.Dtos.SpecLookups;

public sealed class CachedSpecLookupData : LocallyCachedObject
{
    public CachedSpecLookupData( IReadOnlyList<int> globalIds, IReadOnlyDictionary<PrimaryCategory, List<int>> categories, IReadOnlyDictionary<int, SpecLookupResponse> responses )
    {
        GlobalSpecIds = globalIds;
        SpecIdsByCategory = categories;
        ResponsesBySpecId = responses;
    }

    public IReadOnlyList<int> GlobalSpecIds { get; init; }
    public IReadOnlyDictionary<PrimaryCategory, List<int>> SpecIdsByCategory { get; init; }
    public IReadOnlyDictionary<int, SpecLookupResponse> ResponsesBySpecId { get; init; }
    
    public List<SpecLookupResponse> GetGlobalResponses()
    {
        return ( from specId in GlobalSpecIds select ResponsesBySpecId[ specId ] ).ToList();
    }
    public List<SpecLookupResponse> GetResponsesByCategory( PrimaryCategory category )
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