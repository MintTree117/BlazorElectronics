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

        SpecLookupsModel? model;

        try
        {
            model = await _repository.Get();
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ApiReply<SpecLookupsResponse?>( ServiceErrorType.ServerError );
        }

        if ( model is null )
            return new ApiReply<SpecLookupsResponse?>( ServiceErrorType.NotFound );

        SpecLookupsResponse? response = MapResponse( model );

        if ( response is null )
            return new ApiReply<SpecLookupsResponse?>( ServiceErrorType.NotFound );

        _cachedSpecData = new CachedObject<SpecLookupsResponse>( response );
        
        return new ApiReply<SpecLookupsResponse?>( response );
    }
    public async Task<ApiReply<SpecLookupViewResponse?>> GetView()
    {
        IEnumerable<SpecLookupModel>? models;

        try
        {
            models = await _repository.GetView();
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ApiReply<SpecLookupViewResponse?>( ServiceErrorType.ServerError );
        }

        SpecLookupViewResponse? dto = MapView( models );

        return dto is not null
            ? new ApiReply<SpecLookupViewResponse?>( dto )
            : new ApiReply<SpecLookupViewResponse?>( ServiceErrorType.NotFound );
    }
    public async Task<ApiReply<SpecLookupEditDto?>> GetEdit( int specId )
    {
        SpecLookupEditModel? model;

        try
        {
            model = await _repository.GetEdit( specId );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ApiReply<SpecLookupEditDto?>( ServiceErrorType.ServerError );
        }

        SpecLookupEditDto? dto = MapEdit( model );

        return dto is not null
            ? new ApiReply<SpecLookupEditDto?>( dto )
            : new ApiReply<SpecLookupEditDto?>( ServiceErrorType.NotFound );
    }
    public async Task<ApiReply<int>> Add( SpecLookupEditDto dto )
    {
        try
        {
            int result = await _repository.Insert( dto );

            return result > 0
                ? new ApiReply<int>( result )
                : new ApiReply<int>( ServiceErrorType.NotFound );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ApiReply<int>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ApiReply<bool>> Update( SpecLookupEditDto dto )
    {
        try
        {
            bool result = await _repository.Update( dto );

            return result
                ? new ApiReply<bool>( result )
                : new ApiReply<bool>( ServiceErrorType.NotFound );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ApiReply<bool>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ApiReply<bool>> Remove( int specId )
    {
        try
        {
            bool result = await _repository.Delete( specId );

            return result
                ? new ApiReply<bool>( result )
                : new ApiReply<bool>( ServiceErrorType.NotFound );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ApiReply<bool>( ServiceErrorType.ServerError );
        }
    }
    
    static SpecLookupsResponse? MapResponse( SpecLookupsModel model )
    {
        if ( model.GlobalSpecs is null || model.SpecCategories is null || model.SpecLookups is null || model.SpecValues is null )
            return null;

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

        return new SpecLookupsResponse( globalIds, idsByCategory, responsesById );
    }
    static SpecLookupViewResponse? MapView( IEnumerable<SpecLookupModel>? models )
    {
        if ( models is null )
            return null;

        List<SpecLookupViewDto> dtos = models
            .Select( m => new SpecLookupViewDto { SpecId = m.SpecId, SpecName = m.SpecName } )
            .ToList();

        return new SpecLookupViewResponse
        {
            Lookups = dtos
        };
    }
    static SpecLookupEditDto? MapEdit( SpecLookupEditModel? model )
    {
        if ( model?.Spec is null )
            return null;

        string categories = model.Categories is not null
            ? ConvertPrimaryCategoriesToString( model.Categories.Select( c => c.PrimaryCategory ) )
            : string.Empty;

        string values = model.Values is not null
            ? ConvertSpecValuesToString( model.Values )
            : string.Empty;

        return new SpecLookupEditDto
        {
            SpecId = model.Spec.SpecId,
            SpecName = model.Spec.SpecName,
            PrimaryCategoriesAsString = categories,
            ValuesByIdAsString = values
        };
    }
    bool CacheValid()
    {
        return _cachedSpecData is not null && _cachedSpecData.IsValid( MAX_HOURS_BEFORE_CACHE_INVALIDATION );
    }
}