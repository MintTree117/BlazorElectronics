using BlazorElectronics.Server.Models.Categories;
using BlazorElectronics.Server.Repositories.Categories;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DataTransferObjects.Categories;

namespace BlazorElectronics.Server.Services.Categories;

public class CategoryService : ICategoryService
{
    readonly ICategoryCache _cache;
    readonly ICategoryRepository _repository;

    public CategoryService( ICategoryCache cache, ICategoryRepository repository )
    {
        _cache = cache;
        _repository = repository;
    }

    public async Task<ServiceResponse<Categories_DTO?>> GetCategoriesDto()
    {
        ServiceResponse<CategoryMeta?> meta = await GetCategoryMeta();

        if ( meta.Data == null )
            return new ServiceResponse<Categories_DTO?>( meta.Message );

        Categories_DTO dto = await MapMetaToDto( meta.Data );

        return new ServiceResponse<Categories_DTO?>( dto, true, "Successfully got categories dto." );
    }
    public async Task<ServiceResponse<int>> GetCategoryIdFromUrl( string url )
    {
        ServiceResponse<CategoryMeta?> serviceResponse = await GetCategoryMeta();

        if ( serviceResponse.Data == null || !serviceResponse.IsSuccessful() )
            return new ServiceResponse<int>( serviceResponse.Message );

        return !serviceResponse.Data.CategoryIdsByUrl.TryGetValue( url, out int id ) 
            ? new ServiceResponse<int>( "Invalid Category Url!" ) 
            : new ServiceResponse<int>( id, true, "Successfully validated category id by url." );
    }

    async Task<ServiceResponse<CategoryMeta?>> GetCategoryMeta()
    {
        CategoryMeta? meta = await _cache.Get();

        if ( meta != null )
            return new ServiceResponse<CategoryMeta?>( meta, true, "Success. Retrieved Category Meta from cache." );

        IEnumerable<Category>? models = await _repository.GetAll();

        if ( models == null )
            return new ServiceResponse<CategoryMeta?>( null, false, "Failed to retrieve Category Meta from cache, and Categories from repository!" );

        meta = await MapModelsToMeta( models );
        await _cache.Set( meta );

        return new ServiceResponse<CategoryMeta?>( meta, true, "Successfully retrieved Category Meta from repository, mapped to DTO, and cached." );
    }
    static async Task<CategoryMeta> MapModelsToMeta( IEnumerable<Category> categories )
    {
        var meta = new CategoryMeta();

        await Task.Run( () =>
        {
            foreach ( Category c in categories )
            {
                if ( meta.CategoriesById.ContainsKey( c.CategoryId ) )
                    continue;

                if ( string.IsNullOrEmpty( c.CategoryName ) || string.IsNullOrEmpty( c.CategoryUrl ) )
                    continue;

                var categoryDTO = new Category_DTO {
                    Id = c.CategoryId,
                    Name = c.CategoryName,
                    Url = c.CategoryUrl,
                    ImageUrl = c.CategoryImageUrl ?? string.Empty,
                };

                foreach ( CategorySub cs in c.SubCategories )
                    categoryDTO.SubCategoryIds.Add( cs.CategoryId );

                meta.CategoriesById.Add( categoryDTO.Id, categoryDTO );
                meta.CategoryIdsByUrl.Add( categoryDTO.Url, categoryDTO.Id );

                if ( c.IsPrimaryCategory )
                    meta.PrimaryCategoryIds.Add( c.CategoryId );
            }
        } );

        return meta;
    }
    static async Task<Categories_DTO> MapMetaToDto( CategoryMeta meta )
    {
        var dto = new Categories_DTO();

        await Task.Run( () =>
        {
            dto.CategoriesById = meta.CategoriesById;
            dto.PrimaryCategoryIds = meta.PrimaryCategoryIds;
        } );
        
        return dto;
    }
}