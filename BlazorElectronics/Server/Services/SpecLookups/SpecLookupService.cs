using BlazorElectronics.Server.Dtos;
using BlazorElectronics.Server.Models.SpecLookups;
using BlazorElectronics.Server.Repositories;
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
    
    public async Task<ServiceReply<SpecLookupsResponse?>> GetLookups( List<int> primaryCategories )
    {
        if ( CacheValid() )
            return new ServiceReply<SpecLookupsResponse?>( _cachedSpecData!.Object );

        try
        {
            SpecLookupsModel? model = await _repository.Get();
            SpecLookupsResponse? response = MapResponse( model, primaryCategories );

            _cachedSpecData = response is not null ? new CachedObject<SpecLookupsResponse>( response ) : null;

            return response is not null 
                ? new ServiceReply<SpecLookupsResponse?>( response ) 
                : new ServiceReply<SpecLookupsResponse?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<SpecLookupsResponse?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<List<CrudView>?>> GetView()
    {
        try
        {
            IEnumerable<SpecLookupModel>? models = await _repository.GetView();
            List<CrudView>? dto = MapView( models );

            return dto is not null
                ? new ServiceReply<List<CrudView>?>( dto )
                : new ServiceReply<List<CrudView>?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<List<CrudView>?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<SpecLookupEdit?>> GetEdit( int specId )
    {
        try
        {
            SpecLookupEditModel? model = await _repository.GetEdit( specId );
            SpecLookupEdit? dto = MapEdit( model );

            return dto is not null
                ? new ServiceReply<SpecLookupEdit?>( dto )
                : new ServiceReply<SpecLookupEdit?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<SpecLookupEdit?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<int>> Add( SpecLookupEdit dto )
    {
        try
        {
            int id = await _repository.Insert( dto );

            return id > 0
                ? new ServiceReply<int>( id )
                : new ServiceReply<int>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<int>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<bool>> Update( SpecLookupEdit dto )
    {
        try
        {
            bool result = await _repository.Update( dto );

            return result
                ? new ServiceReply<bool>( result )
                : new ServiceReply<bool>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<bool>> Remove( int specId )
    {
        try
        {
            bool result = await _repository.Delete( specId );

            return result
                ? new ServiceReply<bool>( result )
                : new ServiceReply<bool>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
    }
    
    static SpecLookupsResponse? MapResponse( SpecLookupsModel? model, List<int> primaryIds )
    {
        if ( model?.GlobalSpecs is null || model.SpecCategories is null || model.SpecLookups is null || model.SpecValues is null )
            return null;

        List<int> globalIds = model.GlobalSpecs.ToList();
        Dictionary<int, List<int>> idsByCategory = primaryIds.ToDictionary( id => id, id => new List<int>() );

        foreach ( SpecLookupCategoryModel sc in model.SpecCategories )
        {
            idsByCategory[ sc.PrimaryCategoryId ].Add( sc.SpecId );
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

        return new SpecLookupsResponse( globalIds, idsByCategory, responsesById );
    }
    static List<CrudView>? MapView( IEnumerable<SpecLookupModel>? models )
    {
        if ( models is null )
            return null;

        return models
            .Select( m => new CrudView { Id = m.SpecId, Name = m.SpecName } )
            .ToList();
    }
    static SpecLookupEdit? MapEdit( SpecLookupEditModel? model )
    {
        if ( model?.Spec is null )
            return null;

        List<int> categories = model.Categories is not null
            ? model.Categories.Select( c => c.PrimaryCategoryId ).ToList()
            : new List<int>();

        string values = model.Values is not null
            ? ConvertSpecValuesToString( model.Values )
            : string.Empty;

        return new SpecLookupEdit
        {
            SpecId = model.Spec.SpecId,
            SpecName = model.Spec.SpecName,
            PrimaryCategories = categories,
            ValuesByIdAsString = values
        };
    }
    bool CacheValid()
    {
        return _cachedSpecData is not null && _cachedSpecData.IsValid( MAX_HOURS_BEFORE_CACHE_INVALIDATION );
    }
}