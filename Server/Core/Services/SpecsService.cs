using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Server.Core.Models;
using BlazorElectronics.Server.Core.Models.SpecLookups;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.Specs;

namespace BlazorElectronics.Server.Core.Services;

public sealed class SpecsService : ApiService, ISpecsService
{
    const int MAX_HOURS_BEFORE_CACHE_INVALIDATION = 4;
    CachedObject<LookupSpecsDto>? _cachedSpecData;

    readonly ISpecRepository _repository;
    
    public SpecsService( ILogger<ApiService> logger, ISpecRepository repository )
        : base( logger )
    {
        _repository = repository;
    }
    
    public async Task<ServiceReply<LookupSpecsDto?>> GetSpecs( List<int> primaryCategoryIds )
    {
        if ( CacheValid() )
            return new ServiceReply<LookupSpecsDto?>( _cachedSpecData!.Object );

        try
        {
            SpecsModel? model = await _repository.Get();
            LookupSpecsDto? response = MapResponse( model, primaryCategoryIds );

            _cachedSpecData = response is not null ? new CachedObject<LookupSpecsDto>( response ) : null;

            return response is not null 
                ? new ServiceReply<LookupSpecsDto?>( response ) 
                : new ServiceReply<LookupSpecsDto?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<LookupSpecsDto?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<List<CrudViewDto>?>> GetView()
    {
        try
        {
            IEnumerable<SpecModel>? models = await _repository.GetView();
            List<CrudViewDto>? dto = MapView( models );

            return dto is not null
                ? new ServiceReply<List<CrudViewDto>?>( dto )
                : new ServiceReply<List<CrudViewDto>?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<List<CrudViewDto>?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<LookupSpecEditDto?>> GetEdit( int specId )
    {
        try
        {
            SpecEditModel? model = await _repository.GetEdit( specId );
            LookupSpecEditDto? dto = MapEdit( model );

            return dto is not null
                ? new ServiceReply<LookupSpecEditDto?>( dto )
                : new ServiceReply<LookupSpecEditDto?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<LookupSpecEditDto?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<int>> Add( LookupSpecEditDto dto )
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
    public async Task<ServiceReply<bool>> Update( LookupSpecEditDto dto )
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
    
    static LookupSpecsDto? MapResponse( SpecsModel? model, List<int> primaryCategoryIds )
    {
        if ( model?.SpecCategories is null || model.Specs is null || model.SpecValues is null )
            return null;

        List<int> globalIds = new();
        Dictionary<int, List<int>> idsByCategory = new();

        foreach ( int c in primaryCategoryIds )
        {
            idsByCategory.TryAdd( c, new List<int>() );
        }

        foreach ( SpecCategoryModel sc in model.SpecCategories )
        {
            idsByCategory[ sc.PrimaryCategoryId ].Add( sc.SpecId );
        }

        var responsesById = new Dictionary<int, LookupSpec>();

        foreach ( SpecModel m in model.Specs )
        {
            if ( m.IsGlobal )
                globalIds.Add( m.SpecId );
            
            List<string> specValues = model.SpecValues
                .Where( spec => spec.SpecId == m.SpecId )
                .OrderBy( spec => spec.SpecValueId )
                .Select( spec => spec.SpecValue )
                .ToList();

            responsesById.Add( m.SpecId, new LookupSpec
            {
                SpecId = m.SpecId,
                IsAvoid = m.IsAvoid,
                SpecName = m.SpecName,
                Values = specValues
            } );
        }

        return new LookupSpecsDto( globalIds, idsByCategory, responsesById );
    }
    static List<CrudViewDto>? MapView( IEnumerable<SpecModel>? models )
    {
        if ( models is null )
            return null;

        return models
            .Select( m => new CrudViewDto { Id = m.SpecId, Name = m.SpecName } )
            .ToList();
    }
    static LookupSpecEditDto? MapEdit( SpecEditModel? model )
    {
        if ( model?.Spec is null )
            return null;

        List<int> categories = model.Categories is not null
            ? model.Categories.Select( c => c.PrimaryCategoryId ).ToList()
            : new List<int>();

        string values = model.Values is not null
            ? ConvertSpecValuesToString( model.Values )
            : string.Empty;

        return new LookupSpecEditDto
        {
            SpecId = model.Spec.SpecId,
            SpecName = model.Spec.SpecName,
            IsGlobal = model.Spec.IsGlobal,
            IsAvoid = model.Spec.IsAvoid,
            PrimaryCategories = categories,
            ValuesByIdAsString = values
        };
    }
    static string ConvertSpecValuesToString( IEnumerable<SpecValueModel> values )
    {
        List<string> specValues = values
            .OrderBy( spec => spec.SpecValueId )
            .Select( spec => spec.SpecValue )
            .ToList();

        return string.Join( ",", specValues );
    }
    bool CacheValid()
    {
        return _cachedSpecData is not null && _cachedSpecData.IsValid( MAX_HOURS_BEFORE_CACHE_INVALIDATION );
    }
}