using BlazorElectronics.Server.Models.Categories;
using BlazorElectronics.Server.Repositories.Categories;
using BlazorElectronics.Shared.DataTransferObjects.Categories;

namespace BlazorElectronics.Server.Services.Categories;

public class CategoryService : ICategoryService
{
    readonly ICategoryRepository _repository;
    readonly ICategoryCache _cache;

    public CategoryService( ICategoryRepository repository, ICategoryCache cache )
    {
        _repository = repository;
        _cache = cache;
    }
    
    public async Task<ServiceResponse<Categories_DTO?>> GetCategories()
    {
        List<Category>? categories = await TryGetCategories();

        if ( categories == null )
            return new ServiceResponse<Categories_DTO?>( null, false, "Failed to get categories from cache or database!" );

        var categoriesDto = new Categories_DTO();

        await Task.Run( () => {
            foreach ( Category c in categories ) {
                var categoryDTO = new Category_DTO {
                    Id = c.CategoryId,
                    Name = c.CategoryName ?? string.Empty,
                    Url = c.CategoryUrl ?? string.Empty,
                    ImageUrl = c.CategoryImageUrl ?? string.Empty,
                    IsPrimary = c.IsPrimaryCategory
                };
                foreach ( CategorySub cs in c.SubCategories ) {
                    categoryDTO.SubCategories.Add( new CategorySub_DTO {
                        CategoryId = cs.CategoryId,
                        PrimaryCategoryId = cs.PrimaryCategoryId
                    } );
                }
                categoriesDto.Categories.Add( categoryDTO );
            }
        } );

        return new ServiceResponse<Categories_DTO?>( categoriesDto, true, "Successfully retrieved category Dto's from repository." );
    }
    public async Task<ServiceResponse<int>> CategoryIdFromUrl( string categoryUrl )
    {
        if ( !_cache.HasCategoryUrls() || await _cache.GetCategories() == null )
            return new ServiceResponse<int>( -1, false, "Failed to get Categories!" );
        if ( !_cache.UrlToCategoryId( categoryUrl, out int id ) )
            return new ServiceResponse<int>( -1, false, "Failed to get Categories!" );
        return new ServiceResponse<int>( id, true, "Successfully validated url to category id." );
    }

    async Task<List<Category>?> TryGetCategories()
    {
        List<Category>? categories = await _cache.GetCategories();
        
        if ( categories != null ) 
            return categories;
        
        categories = await _repository.GetCategories();
        
        if ( categories == null )
            return null;
        
        await _cache.CacheCategories( categories );
        return categories;
    }
}