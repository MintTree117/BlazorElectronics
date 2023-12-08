using BlazorElectronics.Server.Dtos;
using BlazorElectronics.Server.Models.SpecLookups;
using BlazorElectronics.Server.Repositories;
using BlazorElectronics.Server.Repositories.Specs;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.SpecLookups;

namespace BlazorElectronics.Server.Services.Specs;

public sealed class SpecsService : ApiService, ISpecsService
{
    const int MAX_HOURS_BEFORE_CACHE_INVALIDATION = 4;
    CachedObject<SpecsResponse>? _cachedSpecData;

    readonly ISpecRepository _repository;
    
    public SpecsService( ILogger<ApiService> logger, ISpecRepository repository )
        : base( logger )
    {
        _repository = repository;
    }
    
    public async Task<ServiceReply<SpecsResponse?>> GetLookups( List<int> primaryCategories )
    {
        if ( CacheValid() )
            return new ServiceReply<SpecsResponse?>( _cachedSpecData!.Object );

        try
        {
            SpecsModel? model = await _repository.Get();
            SpecsResponse? response = MapResponse( model, primaryCategories );

            _cachedSpecData = response is not null ? new CachedObject<SpecsResponse>( response ) : null;

            return response is not null 
                ? new ServiceReply<SpecsResponse?>( response ) 
                : new ServiceReply<SpecsResponse?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<SpecsResponse?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<List<CrudView>?>> GetView()
    {
        try
        {
            IEnumerable<SpecModel>? models = await _repository.GetView();
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
    public async Task<ServiceReply<SpecEdit?>> GetEdit( int specId )
    {
        try
        {
            SpecEditModel? model = await _repository.GetEdit( specId );
            SpecEdit? dto = MapEdit( model );

            return dto is not null
                ? new ServiceReply<SpecEdit?>( dto )
                : new ServiceReply<SpecEdit?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<SpecEdit?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<int>> Add( SpecEdit dto )
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
    public async Task<ServiceReply<bool>> Update( SpecEdit dto )
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
    
    static SpecsResponse? MapResponse( SpecsModel? model, IEnumerable<int> primaryIds )
    {
        if ( model?.SpecCategories is null || model.Specs is null || model.SpecValues is null )
            return null;

        List<int> globalIds = new();
        Dictionary<int, List<int>> idsByCategory = primaryIds.ToDictionary( id => id, id => new List<int>() );

        foreach ( SpecCategoryModel sc in model.SpecCategories )
        {
            idsByCategory[ sc.PrimaryCategoryId ].Add( sc.SpecId );
        }

        var responsesById = new Dictionary<int, Spec>();

        foreach ( SpecModel m in model.Specs )
        {
            if ( m.IsGlobal )
                globalIds.Add( m.SpecId );
            
            List<string> specValues = model.SpecValues
                .Where( spec => spec.SpecId == m.SpecId )
                .OrderBy( spec => spec.SpecValueId )
                .Select( spec => spec.SpecValue )
                .ToList();

            responsesById.Add( m.SpecId, new Spec
            {
                SpecId = m.SpecId,
                IsAvoid = m.IsAvoid,
                SpecName = m.SpecName,
                Values = specValues
            } );
        }

        return new SpecsResponse( globalIds, idsByCategory, responsesById );
    }
    static List<CrudView>? MapView( IEnumerable<SpecModel>? models )
    {
        if ( models is null )
            return null;

        return models
            .Select( m => new CrudView { Id = m.SpecId, Name = m.SpecName } )
            .ToList();
    }
    static SpecEdit? MapEdit( SpecEditModel? model )
    {
        if ( model?.Spec is null )
            return null;

        List<int> categories = model.Categories is not null
            ? model.Categories.Select( c => c.PrimaryCategoryId ).ToList()
            : new List<int>();

        string values = model.Values is not null
            ? ConvertSpecValuesToString( model.Values )
            : string.Empty;

        return new SpecEdit
        {
            SpecId = model.Spec.SpecId,
            SpecName = model.Spec.SpecName,
            IsGlobal = model.Spec.IsGlobal,
            IsAvoid = model.Spec.IsAvoid,
            PrimaryCategories = categories,
            ValuesByIdAsString = values
        };
    }
    bool CacheValid()
    {
        return _cachedSpecData is not null && _cachedSpecData.IsValid( MAX_HOURS_BEFORE_CACHE_INVALIDATION );
    }
}