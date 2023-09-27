using BlazorElectronics.Server.Models.Specs;
using BlazorElectronics.Server.Repositories.Specs;
using BlazorElectronics.Shared.DataTransferObjects.Specs;

namespace BlazorElectronics.Server.Services.Specs;

public class SpecService : ISpecService
{
    readonly ISpecCache _cache;
    readonly ISpecRepository _repository;

    public SpecService( ISpecCache cache, ISpecRepository repository )
    {
        _cache = cache;
        _repository = repository;
    }

    public async Task<ServiceResponse<Specs_DTO?>> GetSpecsDTO()
    {
        Specs_DTO? dto = await _cache.GetSpecs();

        if ( dto != null )
            return new ServiceResponse<Specs_DTO?>( dto, true, "Success. Retrieved Specs_DTO from cache." );

        IEnumerable<Spec>? models = await _repository.GetSpecs();

        if ( models == null )
            return new ServiceResponse<Specs_DTO?>( dto, true, "Failed to retrieve Specs_DTO from cache, and Spec models from repository!" );

        dto = await MapSpecModelsToDtos( models );
        await _cache.CacheSpecs( dto );

        return new ServiceResponse<Specs_DTO?>( dto, true, "Successfully retrieved Categories from repository, mapped to DTO, and cached." );
    }
    public async Task<ServiceResponse<SpecLookups_DTO?>> GetSpecLookupsDTO()
    {
        SpecLookups_DTO? dto = await _cache.GetSpecLookups();

        if ( dto != null )
            return new ServiceResponse<SpecLookups_DTO?>( dto, true, "Success. Retrieved SpecLookups_DTO from cache." );

        IEnumerable<SpecLookup>? models = await _repository.GetSpecLookups();

        if ( models == null )
            return new ServiceResponse<SpecLookups_DTO?>( dto, true, "Failed to retrieve SpecLookups_DTO from cache, and SpecLookup models from repository!" );

        dto = await MapSpecLookupModelsToDtos( models );
        await _cache.CacheSpecLookups( dto );

        return new ServiceResponse<SpecLookups_DTO?>( dto, true, "Successfully retrieved Categories from repository, mapped to DTO, and cached." );
    }

    static async Task<Specs_DTO> MapSpecModelsToDtos( IEnumerable<Spec> models )
    {
        var specsDto = new Specs_DTO();

        await Task.Run( () =>
        {
            foreach ( Spec s in models )
            {
                var dto = new Spec_DTO {
                    Id = s.SpecId,
                    DataType = MapDataTypeIdToType( s.SpecDataId ),
                    IsRaw = s.SpecType == SpecType.Raw,
                    Name = s.SpecName
                };

                foreach ( SpecCategory c in s.SpecCategories )
                {
                    if ( !dto.SpecCategoryIds.Contains( c.SpecCategoryId ) )
                        dto.SpecCategoryIds.Add( c.SpecCategoryId );
                }

                foreach ( SpecFilter f in s.SpecFilters )
                {
                    if ( !dto.SpecFilterIds.Contains( f.SpecFilterId ) )
                        dto.SpecFilterIds.Add( f.SpecFilterId );
                }
            }
        } );

        return specsDto;
    }
    static async Task<SpecLookups_DTO> MapSpecLookupModelsToDtos( IEnumerable<SpecLookup> models )
    {
        var specsDto = new SpecLookups_DTO();

        await Task.Run( () =>
        {
            foreach ( SpecLookup s in models )
            {
                if ( s.LookupValue == null )
                    continue;

                if ( !specsDto.LookupValuesBySpecId.TryGetValue( s.SpecId, out List<object>? values ) )
                {
                    values = new List<object>();
                    specsDto.LookupValuesBySpecId.Add( s.SpecId, values );
                }
                if ( !values.Contains( s.LookupValue ) )
                    values.Add( s.LookupValue );
            }
        } );

        return specsDto;
    }

    static Type? MapDataTypeIdToType( int dataTypeId )
    {
        return ( SpecDataType ) dataTypeId switch {
            SpecDataType.INT => typeof( int ),
            SpecDataType.STRING => typeof( string ),
            SpecDataType.DECIMAL => typeof( decimal ),
            _ => null
        };
    }
}



















