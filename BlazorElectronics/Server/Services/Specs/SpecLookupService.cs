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
    SpecLookupTableMetaDto? _cachedSpecMetaData;

    enum ExplicitSpecValidationType
    {
        INT,
        STRING
    }

    public SpecLookupService( ILogger logger, ISpecLookupRepository repository ) : base( logger )
    {
        _repository = repository;
    }

    public async Task<Reply<SpecLookupTableMetaDto?>> GetSpecLookupMeta()
    {
        return await TryGetSpecLookupMetaDto();
    }
    public async Task<Reply<SpecLookupsResponse?>> GetSpecLookupsResponse( short? primaryCategoryId = null )
    { 
        Reply<SpecLookupTableMetaDto?> metaReply = await TryGetSpecLookupMetaDto();

        if ( !metaReply.Success || metaReply.Data is null )
            return new Reply<SpecLookupsResponse?>( metaReply.Message );
        
        SpecLookupsResponse response = await MapSpecLookupsToResponse( metaReply.Data, primaryCategoryId );

        return new Reply<SpecLookupsResponse?>( response );
    }
    public async Task<Reply<bool>> ValidateProductSearchRequestSpecFilters( ProductSearchRequestSpecFilters? specFilters, short? primaryCategoryId = null )
    {
        if ( specFilters is null )
            return new Reply<bool>( "Spec Filters are null!" );

        Reply<SpecLookupTableMetaDto?> specMetaReply = await GetSpecLookupMeta();

        if ( !specMetaReply.Success || specMetaReply.Data is null )
            return new Reply<bool>( NO_DATA_FOUND_MESSAGE );

        await Task.Run( () =>
        {
            SpecLookupTableMetaDto meta = specMetaReply.Data!;

            HashSet<short>? explicitIntIdsByCategory = null;
            HashSet<short>? explicitStringIdsByCategory = null;
            HashSet<short>? dynamicSpecTablesByCategory = null;

            if ( primaryCategoryId is not null )
            {
                explicitIntIdsByCategory = meta.ExplicitIntIdsByCategory[ primaryCategoryId.Value ];
                explicitStringIdsByCategory = meta.ExplicitStringIdsByCategory[ primaryCategoryId.Value ];
                dynamicSpecTablesByCategory = meta.DynamicTableIdsByCategory[ primaryCategoryId.Value ];
            }

            ValidateExplicitSpecs( ExplicitSpecValidationType.INT, specFilters.ExplicitIntSpecsMinimums, meta.ExplicitIntGlobalIds, explicitIntIdsByCategory );
            ValidateExplicitSpecs( ExplicitSpecValidationType.INT, specFilters.ExplicitIntSpecsMaximums, meta.ExplicitIntGlobalIds, explicitIntIdsByCategory );
            ValidateExplicitSpecs( ExplicitSpecValidationType.STRING, specFilters.ExplicitStringSpecsEqualities, meta.ExplicitIntGlobalIds, explicitStringIdsByCategory );
            ValidateDynamicSpecs( specFilters.DynamicSpecsIncludeIdsByTable, meta.DynamicGlobalTableIds, dynamicSpecTablesByCategory );
        } );

        return new Reply<bool>( true );
    }

    async Task<Reply<SpecLookupTableMetaDto?>> TryGetSpecLookupMetaDto()
    {
        if ( _cachedSpecMetaData is not null && _cachedSpecMetaData.IsValid( MAX_HOURS_BEFORE_CACHE_INVALIDATION ) )
            return new Reply<SpecLookupTableMetaDto?>( _cachedSpecMetaData );

        SpecLookupTableMetaModel? metaModel;

        try
        {
            metaModel = await _repository.GetSpecTableMeta();

            if ( metaModel is null )
                return new Reply<SpecLookupTableMetaDto?>( NO_DATA_FOUND_MESSAGE );
        }
        catch ( ServiceException e )
        {
            _logger.LogError( e, e.Message );
            return new Reply<SpecLookupTableMetaDto?>( INTERNAL_SERVER_ERROR_MESSAGE );
        }

        SpecLookupDataModel? dataModel;

        try
        {
            dataModel = await _repository.GetSpecLookupData( metaModel );

            if ( dataModel is null )
                return new Reply<SpecLookupTableMetaDto?>( NO_DATA_FOUND_MESSAGE );
        }
        catch ( ServiceException e )
        {
            _logger.LogError( e, e.Message );
            return new Reply<SpecLookupTableMetaDto?>( INTERNAL_SERVER_ERROR_MESSAGE );
        }

        _cachedSpecMetaData = await MapSpecTableMetaToDto( metaModel, dataModel );
        
        return new Reply<SpecLookupTableMetaDto?>( _cachedSpecMetaData );
    }
    
    static async Task<SpecLookupTableMetaDto> MapSpecTableMetaToDto( SpecLookupTableMetaModel metaModel, SpecLookupDataModel dataModel )
    {
        var dto = new SpecLookupTableMetaDto();
        
        await Task.Run( () =>
        {
            MapGlobalSpecCategories( metaModel.ExplicitIntGlobalIds, dto.ExplicitIntGlobalIds );
            MapExplicitSpecCategories( metaModel.ExplicitIntCategories, dto.ExplicitIntIdsByCategory );
            MapExplicitSpecCategories( metaModel.ExplicitStringCategories, dto.ExplicitStringIdsByCategory );
            MapExplicitSpecNames( metaModel.ExplicitIntNames, dto.ExplicitIntNames );
            MapExplicitSpecNames( metaModel.ExplicitStringNames, dto.ExplicitStringNames );

            MapGlobalSpecCategories( metaModel.DyanmicGlobalIds, dto.DynamicGlobalTableIds );
            MapDynamicSpecCategories( metaModel.DynamicCategories, dto.DynamicTableIdsByCategory );
            MapDynamicSpecNames( metaModel.DynamicTableMeta, dto.DynamicSpecTableNames, dto.DynamicSpecDisplayNames, dto.DynamicProductTableNames );
            MapDynamicSpecValues( dataModel.DyanmicValuesByTableId, dto.DynamicResponseByTableId );
        } );
        
        return dto;
    }
    static async Task<SpecLookupsResponse> MapSpecLookupsToResponse( SpecLookupTableMetaDto meta, short? primaryCategoryId )
    {
        return await Task.Run( () =>
        {
            var response = new SpecLookupsResponse();

            MapSpecsResponse( response, meta.DynamicGlobalTableIds, meta.DynamicSpecDisplayNames, meta.DynamicResponseByTableId );

            if ( primaryCategoryId is null )
                return response;
            if ( !meta.DynamicTableIdsByCategory.TryGetValue( primaryCategoryId.Value, out HashSet<short>? specTableIdsForCategory ) )
                return response;

            MapSpecsResponse( response, specTableIdsForCategory, meta.DynamicSpecDisplayNames, meta.DynamicResponseByTableId );

            return response;
        } );
    }
    
    static void ValidateExplicitSpecs<T>( ExplicitSpecValidationType valueValidationType, Dictionary<short, T>? explicitSpecs, HashSet<short> explicitGlobalIds, HashSet<short>? explicitIdsByCategory )
    {
        if ( explicitSpecs is null )
            return;

        var specsToRemove = new List<short>();
        
        foreach ( short explicitSpecId in explicitSpecs.Keys )
        {
            if ( !explicitSpecs.TryGetValue( explicitSpecId, out T? specValue ) || specValue is null )
            {
                specsToRemove.Add( explicitSpecId );
                continue;
            }

            bool validateValue = valueValidationType switch {
                ExplicitSpecValidationType.INT => ValidateExplicitIntValue( Convert.ToInt16( specValue ) ),
                ExplicitSpecValidationType.STRING => ValidateExplicitStringValue( specValue.ToString() ),
                _ => throw new ServiceException( "Invalid ExplicitSpecValidationType enum encountered!", null )
            };

            if ( !validateValue )
            {
                specsToRemove.Add( explicitSpecId );
                continue;
            }

            if ( explicitGlobalIds.Contains( explicitSpecId ) ) 
                continue;
            
            if ( explicitIdsByCategory is null )
            {
                specsToRemove.Add( explicitSpecId );
                continue;
            }

            if ( !explicitIdsByCategory.Contains( explicitSpecId ) )
            {
                specsToRemove.Add( explicitSpecId );
            }
        }

        foreach ( short explicitId in specsToRemove )
        {
            explicitSpecs.Remove( explicitId );
        }
    }
    static bool ValidateExplicitIntValue( int specValue )
    {
        return specValue > 0;
    }
    static bool ValidateExplicitStringValue( string? specValue )
    {
        return !string.IsNullOrWhiteSpace( specValue ) && specValue.Length < 64;
    }
    static void ValidateDynamicSpecs( Dictionary<short, List<short>>? requestSpecIdsByTable, IReadOnlySet<short> metaGlobalTableIds, IReadOnlySet<short>? metaTableIdsCategory )
    {
        if ( requestSpecIdsByTable is null )
            return;

        var tablesToRemove = new List<int>();

        foreach ( short tableId in requestSpecIdsByTable.Keys )
        {
            // Check Request Value
            if ( !requestSpecIdsByTable.TryGetValue( tableId, out List<short>? requestSpecIds ) )
            {
                tablesToRemove.Add( tableId );
                continue;
            }

            bool tableExists = metaGlobalTableIds.Contains( tableId ) || ( bool ) metaTableIdsCategory?.Contains( tableId );

            if ( tableExists )
                continue;

            tablesToRemove.Add( tableId );
        }
        
        foreach ( short tableId in tablesToRemove )
        {
            requestSpecIdsByTable.Remove( tableId );
        }
    }

    static void MapSpecsResponse( SpecLookupsResponse response, HashSet<short> tableIds, Dictionary<short, string> displayNames, Dictionary<short, List<SpecLookupValueResponse>> valueResponse )
    {
        foreach ( short tableId in tableIds )
        {
            if ( !displayNames.TryGetValue( tableId, out string? globalDisplayName ) )
                continue;
            if ( !valueResponse.TryGetValue( tableId, out List<SpecLookupValueResponse>? lookupValues ) )
                continue;
            
            response.Lookups.Add(
                new SpecLookupTableResponse( tableId, globalDisplayName, lookupValues ) );
        }
    }
    
    static void MapGlobalSpecCategories( IEnumerable<int>? globalIds, HashSet<short> dto )
    {
        if ( globalIds is null )
            return;

        foreach ( short globalId in globalIds )
        {
            dto.Add( globalId );
        }
    }
    static void MapExplicitSpecCategories( IEnumerable<ExplicitProductSpecCategory>? explicitCategories, Dictionary<short, HashSet<short>> dto )
    {
        if ( explicitCategories is null )
            return;
        
        foreach ( ExplicitProductSpecCategory specCategory in explicitCategories )
        {
            if ( !dto.TryGetValue( specCategory.PrimaryCategoryId, out HashSet<short>? tableIdSet ) )
            {
                tableIdSet = new HashSet<short>();
                dto.Add( specCategory.PrimaryCategoryId, tableIdSet );
            }

            tableIdSet.Add( specCategory.ExplicitSpecId );
        }
    }
    static void MapExplicitSpecNames( IEnumerable<ExplicitProductSpecName>? specNames, Dictionary<short, string> dto )
    {
        if ( specNames is null )
            return;

        foreach ( ExplicitProductSpecName specName in specNames )
        {
            dto.TryAdd( specName.ExplicitSpecId, specName.ExplicitSpecName );
        }
    }
    static void MapDynamicSpecCategories( IEnumerable<DynamicSpecTableCategory>? dyanmicCategories, Dictionary<short, HashSet<short>> dto )
    {
        if ( dyanmicCategories is null )
            return;

        foreach ( DynamicSpecTableCategory specCategory in dyanmicCategories )
        {
            if ( !dto.TryGetValue( specCategory.PrimaryCategoryId, out HashSet<short>? tableIdSet ) )
            {
                tableIdSet = new HashSet<short>();
                dto.Add( specCategory.PrimaryCategoryId, tableIdSet );
            }

            tableIdSet.Add( specCategory.SpecTableId );
        }
    }
    static void MapDynamicSpecNames( IEnumerable<DynamicSpecTableMeta>? dynamicMeta, Dictionary<short, string> specTableNamesDto, Dictionary<short, string> productTableNamesDto, Dictionary<short, string> displayNamesDto )
    {
        if ( dynamicMeta is null )
            return;

        foreach ( DynamicSpecTableMeta tableMeta in dynamicMeta )
        {
            string productSpecTableName = $"Product_{tableMeta.LookupTableName}";

            specTableNamesDto.TryAdd( tableMeta.LookupTableId, tableMeta.LookupTableName );
            productTableNamesDto.TryAdd( tableMeta.LookupTableId, productSpecTableName );
            displayNamesDto.TryAdd( tableMeta.LookupTableId, tableMeta.DisplayName );
        }
    }
    static void MapDynamicSpecValues( Dictionary<int, IEnumerable<DynamicSpecValue>?>? dynamicValueByTable, Dictionary<short, List<SpecLookupValueResponse>> dto )
    {
        if ( dynamicValueByTable is null )
            return;
        
        foreach ( short tableId in dynamicValueByTable.Keys )
        {
            if ( dynamicValueByTable[ tableId ] is null || dto.ContainsKey( tableId ) )
                continue;

            var responses = new List<SpecLookupValueResponse>();
            dto.Add( tableId, responses );

            foreach ( DynamicSpecValue value in dynamicValueByTable[ tableId ]! )
            {
                responses.Add( new SpecLookupValueResponse( value.SpecId, value.SpecValue ) );
            }
        }
    }
}