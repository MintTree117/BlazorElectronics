using BlazorElectronics.Server.Models.Specs;
using BlazorElectronics.Server.Repositories.Specs;
using BlazorElectronics.Server.Services.Categories;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DataTransferObjects.Specs;

namespace BlazorElectronics.Server.Services.Specs;

public class SpecService : ISpecService
{
    readonly ISpecCache _cache;
    readonly ISpecRepository _repository;
    readonly ICategoryService _categoryService;

    public SpecService( ISpecCache cache, ISpecRepository repository, ICategoryService categoryService )
    {
        _cache = cache;
        _repository = repository;
        _categoryService = categoryService;
    }

    public async Task<DtoResponse<SpecFilters_DTO>> GetSpecFilters( string categoryUrl )
    {
        Task<DtoResponse<int>> categoryTask = _categoryService.GetCategoryIdFromUrl( categoryUrl );
        Task<CachedSpecDescrs?> descrsTask = GetSpecDescrs();
        Task<CachedSpecLookups?> lookupsTask = GetSpecLookups();

        await Task.WhenAll( categoryTask, descrsTask, lookupsTask );

        if ( !categoryTask.Result.Success )
            return new DtoResponse<SpecFilters_DTO>( null, false, "Invalid Category!" );

        if ( descrsTask.Result == null )
            return new DtoResponse<SpecFilters_DTO>( null, false, "Failed to get SpecDescrs from cache and repository!" );

        if ( lookupsTask.Result == null )
            return new DtoResponse<SpecFilters_DTO>( null, false, "Failed to get SpecDescrs from cache and repository!" );

        CachedSpecDescrs? descrs = descrsTask.Result;
        CachedSpecLookups? lookups = lookupsTask.Result;

        var filterDtos = new List<SpecFilter_DTO>();

        if ( !descrs.IdsByCategoryId.TryGetValue( categoryTask.Result.Data, out List<int>? specIds ) )
            return new DtoResponse<SpecFilters_DTO>( null, false, "Invalid Category!" );

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

        return new DtoResponse<SpecFilters_DTO>( new SpecFilters_DTO { Filters = filterDtos }, true, "Successfully retired filters dto." );
    }

    async Task<CachedSpecDescrs?> GetSpecDescrs()
    {
        CachedSpecDescrs? cachedItems = await _cache.GetSpecDescrs();

        if ( cachedItems != null )
            return cachedItems;

        IEnumerable<SpecDescr>? models = await _repository.GetSpecDescrs();

        if ( models == null )
            return null;

        cachedItems = await MapDescrModels( models );
        await _cache.CacheSpecDescrs( cachedItems );
        
        return cachedItems;
    }
    async Task<CachedSpecLookups?> GetSpecLookups()
    {
        CachedSpecLookups? cachedItems = await _cache.GetSpecLookups();

        if ( cachedItems != null )
            return cachedItems;

        IEnumerable<SpecLookup>? models = await _repository.GetSpecLookups();

        if ( models == null )
            return null;

        cachedItems = await MapLookupModels( models );
        await _cache.CacheSpecLookups( cachedItems );

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



















