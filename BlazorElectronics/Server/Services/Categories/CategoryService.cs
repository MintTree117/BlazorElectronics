using BlazorElectronics.Server.Models.Categories;
using BlazorElectronics.Server.Repositories.Categories;
using BlazorElectronics.Shared.DataTransferObjects.Categories;

namespace BlazorElectronics.Server.Services.Categories;

public class CategoryService : ICategoryService
{
    readonly ICategoryRepository _repository;

    public CategoryService( ICategoryRepository repository )
    {
        _repository = repository;
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
        return new ServiceResponse<int>( 0, true, "Successfully validated url to category id." );
    }

    async Task<List<Category>?> TryGetCategories()
    {
        List<Category>? categories = await _repository.GetCategories();
        
        if ( categories != null ) 
            return categories;
        
        categories = await _repository.GetCategories();
        
        if ( categories == null )
            return null;
        
        return categories;
    }
}