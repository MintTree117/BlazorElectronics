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
    public SpecLookupService( ILogger logger, ISpecLookupRepository repository ) : base( logger )
    {
        _repository = repository;
    }
    
    // PUBLIC API
    public async Task<Reply<CachedSpecData?>> GetSpecDataDto()
    {
        return await TryGetSpecData();
    }
    public async Task<Reply<SpecFiltersResponse?>> GetSpecFiltersResponse( short? primaryCategoryId = null )
    {
        Reply<CachedSpecData?> dataReply = await TryGetSpecData();

        if ( !dataReply.Success || dataReply.Data is null )
            return new Reply<SpecFiltersResponse?>( dataReply.Message );
        
        SpecFiltersResponse filtersResponse = await MapCachedSpecDataToFilters( dataReply.Data, primaryCategoryId );

        return new Reply<SpecFiltersResponse?>( filtersResponse );
    }
    public async Task<Reply<bool>> ValidateProductSearchRequestSpecFilters( ProductSearchRequestSpecFilters specFilters, short? primaryCategoryId = null )
    {
        Reply<CachedSpecData?> dataReply = await GetSpecDataDto();

        if ( !dataReply.Success || dataReply.Data is null )
            return new Reply<bool>( NO_DATA_FOUND_MESSAGE );

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
                dynamicTableCategories = specData.DynamicTablesByCategory[ primaryCategoryId.Value ];
            }

            ValidateFilterLookups( specFilters.IntFilterIds, specData.IntGlobalIds, intIdCategories );
            ValidateFilterLookups( specFilters.StringSpecIds, specData.StringGlobalIds, stringIdCategories );
            ValidateFilterLookups( specFilters.BoolSpecIds, specData.BoolGlobalIds, boolIdCategories );
            ValidateFilterLookups( specFilters.DynamicSpecsIncludeIdsByTable, specData.DynamicGlobalTableIds, dynamicTableCategories );
        } );

        return new Reply<bool>( true );
    }
    
    // FETCH SPEC DATA
    async Task<Reply<CachedSpecData?>> TryGetSpecData()
    {
        if ( _cachedSpecData is not null && _cachedSpecData.IsValid( MAX_HOURS_BEFORE_CACHE_INVALIDATION ) )
            return new Reply<CachedSpecData?>( _cachedSpecData );

        Reply<SpecLookupMetaModel?> metaReply = await GetSpecLookupMetaModel();

        if ( !metaReply.Success || metaReply.Data is null )
            return new Reply<CachedSpecData?>( metaReply.Message );

        SpecDataDto dto = await MapSpecMetaToDto( metaReply.Data );

        Reply<DynamicSpecLookupValuesModel?> valuesReply = await GetDynamicSpecLookupValuesModel( dto.DynamicTableNames );

        if ( !valuesReply.Success || valuesReply.Data?.DyanmicValuesByTableId is null )
            return new Reply<CachedSpecData?>( valuesReply.Message );

        await MapDynamicSpecValues( valuesReply.Data.DyanmicValuesByTableId, dto.DynamicValuesByTable );

        _cachedSpecData = new CachedSpecData( dto );
        return new Reply<CachedSpecData?>( _cachedSpecData );
    }
    async Task<Reply<SpecLookupMetaModel?>> GetSpecLookupMetaModel()
    {
        SpecLookupMetaModel? metaModel;

        try
        {
            metaModel = await _repository.GetSpecLookupMeta();

            if ( metaModel is null )
                return new Reply<SpecLookupMetaModel?>( NO_DATA_FOUND_MESSAGE );
        }
        catch ( ServiceException e )
        {
            _logger.LogError( e, e.Message );
            return new Reply<SpecLookupMetaModel?>( INTERNAL_SERVER_ERROR_MESSAGE );
        }

        return new Reply<SpecLookupMetaModel?>( metaModel );
    }
    async Task<Reply<DynamicSpecLookupValuesModel?>> GetDynamicSpecLookupValuesModel( Dictionary<short, string> dynamicTableNamesById )
    {
        DynamicSpecLookupValuesModel? valuesModel;

        try
        {
            valuesModel = await _repository.GetSpecLookupData( dynamicTableNamesById );

            if ( valuesModel?.DyanmicValuesByTableId is null )
                return new Reply<DynamicSpecLookupValuesModel?>( NO_DATA_FOUND_MESSAGE );
        }
        catch ( ServiceException e )
        {
            _logger.LogError( e, e.Message );
            return new Reply<DynamicSpecLookupValuesModel?>( INTERNAL_SERVER_ERROR_MESSAGE );
        }

        return new Reply<DynamicSpecLookupValuesModel?>( valuesModel );
    }
    static async Task<SpecDataDto> MapSpecMetaToDto( SpecLookupMetaModel metaModel )
    {
        var dto = new SpecDataDto();

        await Task.Run( () =>
        {
            MapGlobalSpecCategories( metaModel.IntGlobalIds, dto.IntGlobalIds );
            MapGlobalSpecCategories( metaModel.StringGlobalIds, dto.IntGlobalIds );
            MapGlobalSpecCategories( metaModel.BoolGlobalIds, dto.BoolGlobalIds );
            
            MapRawSpecCategories( metaModel.IntCategories, dto.IntIdsByCategory );
            MapRawSpecCategories( metaModel.StringCategories, dto.StringIdsByCategory );
            MapRawSpecCategories( metaModel.BoolCategories, dto.BoolIdsByCategory );
            
            MapRawSpecNames( metaModel.IntNames, dto.IntNames );
            MapRawSpecNames( metaModel.StringNames, dto.StringNames );
            MapRawSpecNames( metaModel.BoolNames, dto.BoolNames );
            
            MapStringSpecValues( metaModel.StringValues, dto.StringValues );
            MapIntSpecFilters( metaModel.IntFilters, dto.IntFilterValues );
            
            MapGlobalSpecCategories( metaModel.DyanmicGlobalTableIds, dto.DynamicGlobalTableIds );
            MapDynamicSpecCategories( metaModel.DynamicTableCategories, dto.DynamicTableCategories );
            MapDynamicSpecNames( metaModel.DynamicTables, dto.DynamicTableNames, dto.DynamicDisplayNames, dto.DynamicProductTableNames );
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
    static void MapRawSpecCategories( IEnumerable<RawSpecCategoryModel>? categoryModels, Dictionary<short, HashSet<short>> dto )
    {
        if ( categoryModels is null )
            return;
        
        foreach ( RawSpecCategoryModel specCategory in categoryModels )
        {
            if ( !dto.TryGetValue( specCategory.PrimaryCategoryId, out HashSet<short>? tableIdSet ) )
            {
                tableIdSet = new HashSet<short>();
                dto.Add( specCategory.PrimaryCategoryId, tableIdSet );
            }

            tableIdSet.Add( specCategory.ExplicitSpecId );
        }
    }
    static void MapRawSpecNames( IEnumerable<RawSpecNameModel>? nameModels, Dictionary<short, string> dto )
    {
        if ( nameModels is null )
            return;

        foreach ( RawSpecNameModel specName in nameModels )
        {
            dto.TryAdd( specName.ExplicitSpecId, specName.ExplicitSpecName );
        }
    }
    static void MapStringSpecValues( IEnumerable<StringSpecValueModel>? stringModels, Dictionary<short, List<string>> dto )
    {
        if ( stringModels is null )
            return;

        foreach ( StringSpecValueModel stringValue in stringModels )
        {
            if ( stringValue.Value is null )
                continue;

            if ( !dto.TryGetValue( stringValue.SpecId, out List<string>? dtoValues ) )
            {
                dtoValues = new List<string>();
                dto.Add( stringValue.SpecId, dtoValues );
            }

            dtoValues.Add( stringValue.Value );
        }
    }
    static void MapIntSpecFilters( IEnumerable<IntFilterModel>? filterModels, Dictionary<short, List<string>> dto )
    {
        if ( filterModels is null )
            return;

        foreach ( IntFilterModel specFilter in filterModels )
        {
            if ( specFilter.FilterValue is null )
                continue;
            
            if ( !dto.TryGetValue( specFilter.SpecId, out List<string>? dtoFilters ) )
            {
                dtoFilters = new List<string>();
                dto.Add( specFilter.SpecId, dtoFilters );
            }

            dtoFilters.Add( specFilter.FilterValue );
        }
    }
    static void MapDynamicSpecCategories( IEnumerable<DynamicSpecTableCategoryModel>? categoryModels, Dictionary<short, HashSet<short>> dto )
    {
        if ( categoryModels is null )
            return;

        foreach ( DynamicSpecTableCategoryModel specCategory in categoryModels )
        {
            if ( !dto.TryGetValue( specCategory.PrimaryCategoryId, out HashSet<short>? tableIdSet ) )
            {
                tableIdSet = new HashSet<short>();
                dto.Add( specCategory.PrimaryCategoryId, tableIdSet );
            }

            tableIdSet.Add( specCategory.DynamicTableId );
        }
    }
    static void MapDynamicSpecNames( IEnumerable<DynamicSpecTableMetaModel>? tableMetaModel, Dictionary<short, string> specTableNamesDto, Dictionary<short, string> productTableNamesDto, Dictionary<short, string> displayNamesDto )
    {
        if ( tableMetaModel is null )
            return;

        foreach ( DynamicSpecTableMetaModel tableMeta in tableMetaModel )
        {
            string productSpecTableName = $"Product_{tableMeta.LookupTableName}";

            specTableNamesDto.TryAdd( tableMeta.LookupTableId, tableMeta.LookupTableName );
            productTableNamesDto.TryAdd( tableMeta.LookupTableId, productSpecTableName );
            displayNamesDto.TryAdd( tableMeta.LookupTableId, tableMeta.DisplayName );
        }
    }
    static async Task MapDynamicSpecValues( Dictionary<int, IEnumerable<DynamicSpecValueModel>?> dynamicModels, Dictionary<short, List<string>> dto )
    {
        await Task.Run( () =>
        {
            foreach ( short tableId in dynamicModels.Keys )
            {
                if ( dynamicModels[ tableId ] is null || dto.ContainsKey( tableId ) )
                    continue;

                var values = new List<string>();
                dto.Add( tableId, values );

                foreach ( DynamicSpecValueModel value in dynamicModels[ tableId ]! )
                {
                    values.Add( value.SpecValue );
                }
            }
        } );
    }
    
    // VALIDATE SEARCH REQUEST SPEC FILTERS
    static void ValidateFilterLookups<T>( Dictionary<short, T>? requestFiltersById, IReadOnlySet<short> globalIds, IReadOnlySet<short>? idsByCategory )
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
    static async Task<SpecFiltersResponse> MapCachedSpecDataToFilters( CachedSpecData specData, short? primaryCategoryId )
    {
        return await Task.Run( () =>
        {
            var response = new SpecFiltersResponse();

            MapRawFilterValues( response.IntFilters, specData.IntGlobalIds, specData.IntNames, specData.IntFilterValues );
            MapRawFilterValues( response.StringFilters, specData.StringGlobalIds, specData.StringNames, specData.StringValues );
            MapRawFilterValues( response.DynamicFilters, specData.DynamicGlobalTableIds, specData.DynamicDisplayNames, specData.DynamicValuesByTable );
            MapBoolFilters( response.BoolFilters, specData.BoolGlobalIds, specData.BoolNames );
            
            if ( primaryCategoryId is null )
                return response;

            if ( specData.IntIdsByCategory.TryGetValue( primaryCategoryId.Value, out IReadOnlySet<short>? intIds ) )
                MapRawFilterValues( response.IntFilters, intIds, specData.IntNames, specData.IntFilterValues );

            if ( specData.StringIdsByCategory.TryGetValue( primaryCategoryId.Value, out IReadOnlySet<short>? stringIds ) )
                MapRawFilterValues( response.StringFilters, stringIds, specData.StringNames, specData.StringValues );

            if ( specData.DynamicTablesByCategory.TryGetValue( primaryCategoryId.Value, out IReadOnlySet<short>? dynamicIds ) )
                MapRawFilterValues( response.DynamicFilters, dynamicIds, specData.DynamicDisplayNames, specData.DynamicValuesByTable );

            if ( specData.IntIdsByCategory.TryGetValue( primaryCategoryId.Value, out IReadOnlySet<short>? boolIds ) )
                MapBoolFilters( response.BoolFilters, boolIds, specData.BoolNames );

            return response;
        } );
    }
    static void MapRawFilterValues( List<FilterTableResponse> responses, IReadOnlySet<short> cachedIds, IReadOnlyDictionary<short, string> cachedNames, IReadOnlyDictionary<short, IReadOnlyList<string>> cachedValues )
    {
        foreach ( short id in cachedIds )
        {
            if ( !cachedNames.TryGetValue( id, out string? name ) )
                continue;
            
            if ( !cachedValues.TryGetValue( id, out IReadOnlyList<string>? values ) )
                continue;

            responses.Add( new FilterTableResponse( id, name, values ) );
        }
    }
    static void MapBoolFilters( List<string> responses, IReadOnlySet<short> cachedIds, IReadOnlyDictionary<short, string> cachedNames )
    {
        foreach ( short id in cachedIds )
        {
            if ( !cachedNames.TryGetValue( id, out string? name ) )
                continue;

            responses.Add( name );
        }
    }
}