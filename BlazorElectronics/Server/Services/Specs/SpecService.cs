using BlazorElectronics.Server.Models.Specs;
using BlazorElectronics.Server.Repositories.Specs;
using BlazorElectronics.Server.Services.Categories;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DataTransferObjects.Specs;

namespace BlazorElectronics.Server.Services.Specs;

public class SpecService : ISpecService
{
    readonly ISpecCache _cache;
    readonly ISpecDescrRepository _descrRepository;
    readonly ISpecLookupRepository _lookupRepository;
    readonly ICategoryService _categoryService;

    public SpecService( ISpecCache cache, ISpecLookupRepository lookupRepository, ISpecDescrRepository descrRepository, ICategoryService categoryService )
    {
        _cache = cache;
        _descrRepository = descrRepository;
        _categoryService = categoryService;
        _lookupRepository = lookupRepository;
    }

    public async Task<ServiceResponse<SpecFilters_DTO>> GetSpecFilters( string categoryUrl )
    {
        Task<ServiceResponse<int>> categoryTask = _categoryService.GetCategoryIdFromUrl( categoryUrl );
        
        if ( !categoryTask.Result.Success )
            return new ServiceResponse<SpecFilters_DTO>( null, false, "Invalid Category!" );

        return await GetSpecFilters( categoryTask.Result.Data );
    }
    public async Task<ServiceResponse<SpecFilters_DTO>> GetSpecFilters( int categoryId )
    {
        Task<CachedSpecDescrs?> descrsTask = GetSpecDescrs( categoryId );
        Task<CachedSpecLookups?> lookupsTask = GetSpecLookups( categoryId );

        await Task.WhenAll( descrsTask, lookupsTask );

        if ( descrsTask.Result == null )
            return new ServiceResponse<SpecFilters_DTO>( null, false, "Failed to get SpecDescrs from cache and repository!" );

        if ( lookupsTask.Result == null )
            return new ServiceResponse<SpecFilters_DTO>( null, false, "Failed to get SpecDescrs from cache and repository!" );

        CachedSpecDescrs? descrs = descrsTask.Result;
        CachedSpecLookups? lookups = lookupsTask.Result;

        var filterDtos = new List<SpecFilter_DTO>();

        if ( !descrs.IdsByCategoryId.TryGetValue( categoryId, out List<int>? specIds ) )
            return new ServiceResponse<SpecFilters_DTO>( null, false, "Invalid Category!" );

        foreach ( int id in specIds )
        {
            if ( !descrs.SpecsById.TryGetValue( id, out Spec_DTO? spec ) )
                continue;
            if ( !lookups.SpecLookupsBySpecId.TryGetValue( id, out List<object>? values ) )
                continue;

            filterDtos.Add( new SpecFilter_DTO {
                Id = spec.Id,
                Name = spec.Name,
                Values = values
            } );
        }

        return new ServiceResponse<SpecFilters_DTO>( new SpecFilters_DTO { Filters = filterDtos }, true, "Successfully retired filters dto." );
    }

    async Task<CachedSpecDescrs?> GetSpecDescrs( int categoryId )
    {
        CachedSpecDescrs? cachedItems = await _cache.GetSpecDescrs( categoryId );

        if ( cachedItems != null )
            return cachedItems;

        IEnumerable<SpecDescr>? models = await _descrRepository.GetByCategory( categoryId );

        if ( models == null )
            return null;

        cachedItems = await MapDescrModels( models );
        await _cache.CacheSpecDescrs( cachedItems, categoryId );
        
        return cachedItems;
    }
    async Task<CachedSpecLookups?> GetSpecLookups( int categoryId )
    {
        CachedSpecLookups? cachedItems = await _cache.GetSpecLookups( categoryId );

        if ( cachedItems != null )
            return cachedItems;

        IEnumerable<SpecLookup>? models = await _lookupRepository.GetByCategory( categoryId );

        if ( models == null )
            return null;

        cachedItems = await MapLookupModels( models );
        await _cache.CacheSpecLookups( cachedItems, categoryId );

        return cachedItems;
    }

    static async Task<CachedSpecDescrs> MapDescrModels( IEnumerable<SpecDescr> models )
    {
        var specsDto = new CachedSpecDescrs();

        await Task.Run( () =>
        {
            foreach ( SpecDescr s in models )
            {
                var dto = new Spec_DTO {
                    Id = s.SpecId,
                    Name = s.SpecName,
                    IsDynamic = s.IsDynamic,
                };

                foreach ( SpecCategory c in s.SpecCategories )
                {
                    if ( !dto.SpecCategoryIds.Contains( c.SpecCategoryId ) )
                        dto.SpecCategoryIds.Add( c.SpecCategoryId );
                }
            }
        } );

        return specsDto;
    }
    static async Task<CachedSpecLookups> MapLookupModels( IEnumerable<SpecLookup> lookupsModel )
    {
        var cachedLookups = new CachedSpecLookups();

        await Task.Run( () => { } );

        return cachedLookups;
    }
}



















