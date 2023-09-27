using BlazorElectronics.Server.Models.Categories;
using BlazorElectronics.Server.Repositories.Categories;
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
    
    public async Task<ServiceResponse<Categories_DTO?>> GetCategories()
    {
        Categories_DTO? dto = await _cache.Get();

        if ( dto != null )
            return new ServiceResponse<Categories_DTO?>( dto, true, "Success. Retrieved Categories_DTO from cache." );

        IEnumerable<Category>? models = await _repository.GetCategories();

        if ( models == null )
            return new ServiceResponse<Categories_DTO?>( null, false, "Failed to retrieve Categories_DTO from cache, and Categories from repository!" );

        dto = await MapModelsToDtos( models );
        await _cache.Set( dto );
        
        return new ServiceResponse<Categories_DTO?>( dto, true, "Successfully retrieved Categories from repository, mapped to DTO, and cached." );
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
                dto.CategoryIdsByName.Add( categoryDTO.Name, categoryDTO.Id );
            }
        } );

        return dto;
    }
}