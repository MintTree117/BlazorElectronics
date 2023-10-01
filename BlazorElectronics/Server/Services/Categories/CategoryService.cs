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
    
    public async Task<DtoResponse<Categories_DTO?>> GetCategories()
    {
        Categories_DTO? dto = await _cache.Get();

        if ( dto != null )
            return new DtoResponse<Categories_DTO?>( dto, true, "Success. Retrieved Categories_DTO from cache." );

        IEnumerable<Category>? models = await _repository.GetAll();

        if ( models == null )
            return new DtoResponse<Categories_DTO?>( null, false, "Failed to retrieve Categories_DTO from cache, and Categories from repository!" );

        dto = await MapModelsToDtos( models );
        await _cache.Set( dto );
        
        return new DtoResponse<Categories_DTO?>( dto, true, "Successfully retrieved Categories from repository, mapped to DTO, and cached." );
    }
    public async Task<DtoResponse<int>> GetCategoryIdFromUrl( string url )
    {
        DtoResponse<Categories_DTO?> dtoResponse = await GetCategories();

        if ( !dtoResponse.IsSuccessful() )
            return new DtoResponse<int>( dtoResponse.Message );

        if ( !dtoResponse.Data!.CategoryIdsByUrl.TryGetValue( url, out int id ) )
            return new DtoResponse<int>( "Invalid Category Url!" );

        return new DtoResponse<int>( id, true, "Successfully validated category id by url." );
    }
    static async Task<Categories_DTO> MapModelsToDtos( IEnumerable<Category> categories )
    {
        var dto = new Categories_DTO();

        await Task.Run( () =>
        {
            foreach ( Category c in categories )
            {
                if ( dto.CategoriesById.ContainsKey( c.CategoryId ) )
                    continue;
                
                var categoryDTO = new Category_DTO {
                    Id = c.CategoryId,
                    Name = c.CategoryName ?? string.Empty,
                    Url = c.CategoryUrl ?? string.Empty,
                    ImageUrl = c.CategoryImageUrl ?? string.Empty,
                    IsPrimary = c.IsPrimaryCategory
                };
                foreach ( CategorySub cs in c.SubCategories )
                {
                    categoryDTO.SubCategories.Add( new CategorySub_DTO {
                        CategoryId = cs.CategoryId,
                        PrimaryCategoryId = cs.PrimaryCategoryId
                    } );
                }
                
                dto.CategoriesById.Add( categoryDTO.Id, categoryDTO );
                dto.CategoryIdsByUrl.Add( categoryDTO.Name, categoryDTO.Id );
            }
        } );

        return dto;
    }
}