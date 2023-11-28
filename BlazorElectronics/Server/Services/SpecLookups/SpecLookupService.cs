using BlazorElectronics.Server.Dtos;
using BlazorElectronics.Server.Models.SpecLookups;
using BlazorElectronics.Server.Repositories.SpecLookups;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.SpecLookups;

namespace BlazorElectronics.Server.Services.SpecLookups;

public sealed class SpecLookupService : ApiService, ISpecLookupService
{
    const int MAX_HOURS_BEFORE_CACHE_INVALIDATION = 4;
    CachedObject<SpecLookupsResponse>? _cachedSpecData;

    readonly ISpecLookupRepository _repository;
    
    public SpecLookupService( ILogger<ApiService> logger, ISpecLookupRepository repository )
        : base( logger )
    {
        _repository = repository;
    }
    
    public async Task<ApiReply<SpecLookupsResponse?>> GetLookups()
    {
        if ( CacheValid() )
            return new ApiReply<SpecLookupsResponse?>( _cachedSpecData!.Object );

        SpecLookupsModel? model = await _repository.Get();

        if ( model is null )
            return new ApiReply<SpecLookupsResponse?>( NO_DATA_FOUND_MESSAGE );

        _cachedSpecData = await MapGetResult( model );

        return new ApiReply<SpecLookupsResponse?>( _cachedSpecData!.Object );
    }
    static async Task<CachedObject<SpecLookupsResponse>?> MapGetResult( SpecLookupsModel model )
    {
        if ( model.GlobalSpecs is null || model.SpecCategories is null || model.SpecLookups is null || model.SpecValues is null )
            return null;

        return await Task.Run( () =>
        {
            List<int> globalIds = model.GlobalSpecs.ToList();

            var idsByCategory = new Dictionary<PrimaryCategory, List<int>>
            {
                { PrimaryCategory.BOOKS, new List<int>() },
                { PrimaryCategory.SOFTWARE, new List<int>() },
                { PrimaryCategory.VIDEOGAMES, new List<int>() },
                { PrimaryCategory.MOVIESTV, new List<int>() },
                { PrimaryCategory.COURSES, new List<int>() }
            };
            foreach ( SpecLookupCategoryModel sc in model.SpecCategories )
            {
                idsByCategory[ ( PrimaryCategory ) sc.PrimaryCategory ].Add( sc.SpecId );
            }

            var responsesById = new Dictionary<int, SpecLookupDto>();

            foreach ( SpecLookupModel m in model.SpecLookups )
            {
                List<string> specValues = model.SpecValues
                    .Where( spec => spec.SpecId == m.SpecId )
                    .OrderBy( spec => spec.SpecValueId )
                    .Select( spec => spec.SpecValue )
                    .ToList();

                responsesById.Add( m.SpecId, new SpecLookupDto
                {
                    SpecId = m.SpecId,
                    SpecName = m.SpecName,
                    Values = specValues
                } );
            }

            var response = new SpecLookupsResponse( globalIds, idsByCategory, responsesById );
            return new CachedObject<SpecLookupsResponse>( response );
        } );
    }
    bool CacheValid()
    {
        return _cachedSpecData is not null && _cachedSpecData.IsValid( MAX_HOURS_BEFORE_CACHE_INVALIDATION );
    }
}