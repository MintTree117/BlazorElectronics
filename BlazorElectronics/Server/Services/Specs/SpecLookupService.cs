using BlazorElectronics.Server.Dtos.Specs;
using BlazorElectronics.Server.Models.Specs;
using BlazorElectronics.Server.Repositories.Specs;
using BlazorElectronics.Shared.Inbound.Products;
using BlazorElectronics.Shared.Outbound.Specs;

namespace BlazorElectronics.Server.Services.Specs;

public class SpecLookupService : ApiService, ISpecLookupService
{
    readonly ISpecLookupRepository _repository;

    const int MAX_HOURS_BEFORE_CACHE_INVALIDATION = 4;
    CachedSpecData? _cachedSpecData;
    
    // CONSTRUCTOR
    public SpecLookupService( ILogger<ApiService> logger, ISpecLookupRepository repository ) : base( logger )
    {
        _repository = repository;
    }
    
    // PUBLIC API
    public async Task<ApiReply<CachedSpecData?>> GetSpecDataDto()
    {
        return await TryGetSpecData();
    }
    public async Task<ApiReply<SpecFiltersResponse?>> GetSpecFiltersResponse( short? primaryCategoryId = null )
    {
        ApiReply<CachedSpecData?> dataReply = await TryGetSpecData();

        if ( !dataReply.Success || dataReply.Data is null )
            return new ApiReply<SpecFiltersResponse?>( dataReply.Message );
        
        SpecFiltersResponse filtersResponse = await MapCachedSpecDataToFilterResponse( dataReply.Data, primaryCategoryId );

        return new ApiReply<SpecFiltersResponse?>( filtersResponse );
    }
    public async Task<ApiReply<bool>> ValidateProductSearchRequestSpecFilters( ProductSearchRequestSpecFilters specFilters, short? primaryCategoryId = null )
    {
        ApiReply<CachedSpecData?> dataReply = await GetSpecDataDto();

        if ( !dataReply.Success || dataReply.Data is null )
            return new ApiReply<bool>( NO_DATA_FOUND_MESSAGE );

        await Task.Run( () =>
        {
            CachedSpecData specData = dataReply.Data;

            IReadOnlySet<short>? intIdCategories = null;
            IReadOnlySet<short>? stringIdCategories = null;
            IReadOnlySet<short>? boolIdCategories = null;
            IReadOnlySet<short>? dynamicTableCategories = null;

            if ( primaryCategoryId is not null )
            {
                intIdCategories = specData.IntIdsByCategory[ primaryCategoryId.Value ];
                stringIdCategories = specData.StringIdsByCategory[ primaryCategoryId.Value ];
                boolIdCategories = specData.BoolIdsByCategory[ primaryCategoryId.Value ];
                dynamicTableCategories = specData.MultiIdsByCategory[ primaryCategoryId.Value ];
            }

            ValidateSpecLookupFiltersRequest( specFilters.IntFilters, specData.IntGlobalIds, intIdCategories );
            ValidateSpecLookupFiltersRequest( specFilters.StringFilters, specData.StringGlobalIds, stringIdCategories );
            ValidateSpecLookupFiltersRequest( specFilters.BoolFilters, specData.BoolGlobalIds, boolIdCategories );
            ValidateSpecLookupFiltersRequest( specFilters.MultiIncludes, specData.MultiGlobalIds, dynamicTableCategories );
            ValidateSpecLookupFiltersRequest( specFilters.MultiExcludes, specData.MultiGlobalIds, dynamicTableCategories );
        } );

        return new ApiReply<bool>( true );
    }
    
    // FETCH SPEC DATA
    async Task<ApiReply<CachedSpecData?>> TryGetSpecData()
    {
        if ( _cachedSpecData is not null && _cachedSpecData.IsValid( MAX_HOURS_BEFORE_CACHE_INVALIDATION ) )
            return new ApiReply<CachedSpecData?>( _cachedSpecData );

        ApiReply<SpecLookupsModel?> metaReply = await GetSpecLookupMetaModel();

        if ( !metaReply.Success || metaReply.Data is null )
            return new ApiReply<CachedSpecData?>( metaReply.Message );

        SpecDataDto dto = await MapSpecDataModelToDto( metaReply.Data );

        _cachedSpecData = new CachedSpecData( dto );
        return new ApiReply<CachedSpecData?>( _cachedSpecData );
    }
    async Task<ApiReply<SpecLookupsModel?>> GetSpecLookupMetaModel()
    {
        SpecLookupsModel? metaModel;

        try
        {
            metaModel = await _repository.GetSpecLookupData();

            if ( metaModel is null )
                return new ApiReply<SpecLookupsModel?>( NO_DATA_FOUND_MESSAGE );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e, e.Message );
            return new ApiReply<SpecLookupsModel?>( INTERNAL_SERVER_ERROR_MESSAGE );
        }

        return new ApiReply<SpecLookupsModel?>( metaModel );
    }

    // MAP SPEC DATA
    static async Task<SpecDataDto> MapSpecDataModelToDto( SpecLookupsModel metaModel )
    {
        var dto = new SpecDataDto();

        await Task.Run( () =>
        {
            MapGlobalSpecCategories( metaModel.IntGlobalIds, dto.IntGlobalIds );
            MapGlobalSpecCategories( metaModel.StringGlobalIds, dto.StringGlobalIds );
            MapGlobalSpecCategories( metaModel.BoolGlobalIds, dto.BoolGlobalIds );
            MapGlobalSpecCategories( metaModel.MultiGlobalIds, dto.MultiGlobalIds );
            
            MapSpecCategories( metaModel.IntCategories, dto.IntIdsByCategory );
            MapSpecCategories( metaModel.StringCategories, dto.StringIdsByCategory );
            MapSpecCategories( metaModel.BoolCategories, dto.BoolIdsByCategory );
            MapSpecCategories( metaModel.MultiCategories, dto.MultiIdsByCategory );
            
            MapSpecNames( metaModel.IntNames, dto.IntNames );
            MapSpecNames( metaModel.StringNames, dto.StringNames );
            MapSpecNames( metaModel.BoolNames, dto.BoolNames );
            MapSpecNames( metaModel.MultiNames, dto.MultiNames );

            MapSpecIntFilters( metaModel.IntFilters, dto.IntFilters );
            MapSpecStringValues( metaModel.StringValues, dto.StringValues );
            MapSpecStringValues( metaModel.MultiValues, dto.MultiValues );
        } );

        return dto;
    }
    static void MapGlobalSpecCategories( IEnumerable<short>? ids, HashSet<short> dto )
    {
        if ( ids is null )
            return;

        foreach ( short globalId in ids )
        {
            dto.Add( globalId );
        }
    }
    static void MapSpecCategories( IEnumerable<SpecLookupCategoryModel>? categoryModels, Dictionary<short, HashSet<short>> dto )
    {
        if ( categoryModels is null )
            return;
        
        foreach ( SpecLookupCategoryModel specCategory in categoryModels )
        {
            if ( !dto.TryGetValue( specCategory.PrimaryCategoryId, out HashSet<short>? tableIdSet ) )
            {
                tableIdSet = new HashSet<short>();
                dto.Add( specCategory.PrimaryCategoryId, tableIdSet );
            }

            tableIdSet.Add( specCategory.SpecId );
        }
    }
    static void MapSpecNames( IEnumerable<SpecLookupNameModel>? nameModels, Dictionary<short, string> dto )
    {
        if ( nameModels is null )
            return;

        foreach ( SpecLookupNameModel specName in nameModels )
        {
            dto.TryAdd( specName.SpecId, specName.SpecName );
        }
    }
    static void MapSpecIntFilters( IEnumerable<SpecLookupIntFilterModel>? filterModels, Dictionary<short, List<string>> dto )
    {
        if ( filterModels is null )
            return;
        
        List<SpecLookupIntFilterModel> sortedFilters = filterModels.OrderBy( filter => filter.FilterId ).ToList();

        foreach ( SpecLookupIntFilterModel filterValue in sortedFilters )
        {
            if ( filterValue.FilterValue is null )
                continue;

            string specValue = filterValue.FilterValue;

            if ( string.IsNullOrWhiteSpace( specValue ) )
                continue;

            if ( !dto.TryGetValue( filterValue.SpecId, out List<string>? dtoValues ) )
            {
                dtoValues = new List<string>();
                dto.Add( filterValue.SpecId, dtoValues );
            }

            dtoValues.Add( specValue );
        }
    }
    static void MapSpecStringValues( IEnumerable<SpecLookupStringValueModel>? valueModels, Dictionary<short, List<string>> dto )
    {
        if ( valueModels is null )
            return;

        List<SpecLookupStringValueModel> sortedValues = valueModels.OrderBy( value => value.SpecValueId ).ToList();

        foreach ( SpecLookupStringValueModel stringValue in sortedValues )
        {
            if ( stringValue.SpecValue is null )
                continue;

            string specValue = stringValue.SpecValue;

            if ( string.IsNullOrWhiteSpace( specValue ) )
                continue;

            if ( !dto.TryGetValue( stringValue.SpecId, out List<string>? dtoValues ) )
            {
                dtoValues = new List<string>();
                dto.Add( stringValue.SpecId, dtoValues );
            }

            dtoValues.Add( specValue );
        }
    }

    // VALIDATE SEARCH REQUEST SPEC FILTERS
    static void ValidateSpecLookupFiltersRequest<T>( Dictionary<short, T>? requestFiltersById, IReadOnlySet<short> globalIds, IReadOnlySet<short>? idsByCategory )
    {
        if ( requestFiltersById is null )
            return;
        
        var keysToRemove = new List<short>();

        foreach ( short requestId in requestFiltersById.Keys )
        {
            if ( globalIds.Contains( requestId ) )
                continue;

            if ( idsByCategory is null || !idsByCategory.Contains( requestId ) )
                keysToRemove.Add( requestId );
        }

        foreach ( short keyToRemove in keysToRemove )
            requestFiltersById.Remove( keyToRemove );
    }
    
    // GET SPEC FILTERS FOR CLIENT
    static async Task<SpecFiltersResponse> MapCachedSpecDataToFilterResponse( CachedSpecData specData, short? primaryCategoryId )
    {
        return await Task.Run( () =>
        {
            var response = new SpecFiltersResponse();

            MapFiltersResponse( response.IntFilters, specData.IntGlobalIds, specData.IntNames, specData.IntFilters );
            MapFiltersResponse( response.StringFilters, specData.StringGlobalIds, specData.StringNames, specData.StringValues );
            MapFiltersResponse( response.MultiFilters, specData.MultiGlobalIds, specData.MultiNames, specData.MultiValues );
            MapBoolFiltersResponse( response.BoolFilters, specData.BoolGlobalIds, specData.BoolNames );
            
            if ( primaryCategoryId is null )
                return response;

            if ( specData.IntIdsByCategory.TryGetValue( primaryCategoryId.Value, out IReadOnlySet<short>? intIds ) )
                MapFiltersResponse( response.IntFilters, intIds, specData.IntNames, specData.IntFilters );

            if ( specData.StringIdsByCategory.TryGetValue( primaryCategoryId.Value, out IReadOnlySet<short>? stringIds ) )
                MapFiltersResponse( response.StringFilters, stringIds, specData.StringNames, specData.StringValues );

            if ( specData.MultiIdsByCategory.TryGetValue( primaryCategoryId.Value, out IReadOnlySet<short>? dynamicIds ) )
                MapFiltersResponse( response.MultiFilters, dynamicIds, specData.MultiNames, specData.MultiValues );

            if ( specData.IntIdsByCategory.TryGetValue( primaryCategoryId.Value, out IReadOnlySet<short>? boolIds ) )
                MapBoolFiltersResponse( response.BoolFilters, boolIds, specData.BoolNames );

            return response;
        } );
    }
    static void MapFiltersResponse( List<SpecFilterTableResponse> responses, IReadOnlySet<short> cachedIds, IReadOnlyDictionary<short, string> cachedNames, IReadOnlyDictionary<short, IReadOnlyList<string>> cachedValues )
    {
        foreach ( short id in cachedIds )
        {
            if ( !cachedNames.TryGetValue( id, out string? name ) )
                continue;
            
            if ( !cachedValues.TryGetValue( id, out IReadOnlyList<string>? values ) )
                continue;

            responses.Add( new SpecFilterTableResponse( id, name, values ) );
        }
    }
    static void MapBoolFiltersResponse( List<string> responses, IReadOnlySet<short> cachedIds, IReadOnlyDictionary<short, string> cachedNames )
    {
        foreach ( short id in cachedIds )
        {
            if ( !cachedNames.TryGetValue( id, out string? name ) )
                continue;

            responses.Add( name );
        }
    }
}