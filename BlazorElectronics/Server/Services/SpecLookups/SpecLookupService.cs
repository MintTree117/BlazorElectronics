using BlazorElectronics.Server.Dtos.SpecLookups;
using BlazorElectronics.Server.Models.SpecLookups;
using BlazorElectronics.Server.Repositories.SpecLookups;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.SpecLookups;

namespace BlazorElectronics.Server.Services.SpecLookups;

public class SpecLookupService : ApiService, ISpecLookupService
{
    readonly ISpecLookupRepository _repository;

    const int MAX_HOURS_BEFORE_CACHE_INVALIDATION = 4;
    CachedSpecLookupData? _cachedSpecData;
    
    public SpecLookupService( ILogger<ApiService> logger, ISpecLookupRepository repository ) : base( logger )
    {
        _repository = repository;
    }

    public async Task<ApiReply<CachedSpecLookupData?>> GetSpecLookups()
    {
        ApiReply<CachedSpecLookupData?> getReply = await TryGetSpecLookups();

        if ( !getReply.Success || getReply.Data is null )
            return new ApiReply<CachedSpecLookupData?>( getReply.Message );

        return new ApiReply<CachedSpecLookupData?>( getReply.Data );
    }
    public async Task<ApiReply<List<SpecLookupResponse>?>> GetSpecLookups( PrimaryCategory category )
    {
        ApiReply<CachedSpecLookupData?> getReply = await TryGetSpecLookups();

        if ( !getReply.Success || getReply.Data is null )
            return new ApiReply<List<SpecLookupResponse>?>( getReply.Message );

        return new ApiReply<List<SpecLookupResponse>?>( getReply.Data.GetResponsesByCategory( category ) );
    }

    async Task<ApiReply<CachedSpecLookupData?>> TryGetSpecLookups()
    {
        if ( CacheValid() )
            return new ApiReply<CachedSpecLookupData?>( _cachedSpecData );
        
        SpecLookupsModel? model;
        
        try
        {
            model = await _repository.Get();
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ApiReply<CachedSpecLookupData?>( INTERNAL_SERVER_ERROR_MESSAGE );
        }

        if ( model is null )
            return new ApiReply<CachedSpecLookupData?>( NO_DATA_FOUND_MESSAGE );

        _cachedSpecData = await MapModelToCache( model );

        return _cachedSpecData is not null
            ? new ApiReply<CachedSpecLookupData?>( _cachedSpecData )
            : new ApiReply<CachedSpecLookupData?>( NO_DATA_FOUND_MESSAGE );
    }
    static async Task<CachedSpecLookupData?> MapModelToCache( SpecLookupsModel model )
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

            var responsesById = new Dictionary<int, SpecLookupResponse>();

            foreach ( SpecLookupModel m in model.SpecLookups )
            {
                List<string> specValues = model.SpecValues
                    .Where( spec => spec.SpecId == m.SpecId )
                    .OrderBy( spec => spec.SpecValueId )
                    .Select( spec => spec.SpecValue )
                    .ToList();

                responsesById.Add( m.SpecId, new SpecLookupResponse
                {
                    SpecId = m.SpecId,
                    SpecName = m.SpecName,
                    Values = specValues
                } );
            }

            return new CachedSpecLookupData( globalIds, idsByCategory, responsesById );
        } );
    }

    bool CacheValid()
    {
        return _cachedSpecData is not null && _cachedSpecData.IsValid( MAX_HOURS_BEFORE_CACHE_INVALIDATION );
    }
}