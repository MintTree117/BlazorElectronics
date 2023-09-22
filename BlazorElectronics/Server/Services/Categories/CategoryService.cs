using BlazorElectronics.Server.Models.Categories;
using BlazorElectronics.Server.Repositories.Categories;
using BlazorElectronics.Shared.DataTransferObjects.Categories;

namespace BlazorElectronics.Server.Services.Categories;

public class CategoryService : ICategoryService
{
    readonly ICategoryRepository _categoryRepository;

    public CategoryService( ICategoryRepository categoryRepository )
    {
        _categoryRepository = categoryRepository;
    }
    
    public async Task<ServiceResponse<CategoryLists_DTO?>> GetCategories()
    {
        CategoryCollections? categories = await _categoryRepository.GetCategories();
        
        if ( categories == null )
            return new ServiceResponse<CategoryLists_DTO?>( null, false, "Failed to retrieve products from database!" );

        var categoryDtos = new CategoryLists_DTO();

        await Task.Run( () => {
            foreach ( Category c in categories.Categories ) {
                categoryDtos.PrimaryCategories.Add( new Category_DTO {
                    Id = c.CategoryId,
                    Name = c.CategoryName ?? string.Empty,
                    Url = c.CategoryUrl ?? string.Empty,
                    ImageUrl = c.CategoryImageUrl ?? string.Empty,
                    IsPrimary = c.IsPrimaryCategory
                } );
            }
            foreach ( CategorySub cs in categories.CategoriesSub ) {
                categoryDtos.SubCategories.Add( new CategorySub_DTO {
                    CategoryId = cs.CategoryId,
                    PrimaryCategoryId = cs.PrimaryCategoryId
                } );
            }
        } );

        return new ServiceResponse<CategoryLists_DTO?>( categoryDtos, true, "Successfully retrieved category Dto's from repository." );
    }
    public async Task<ServiceResponse<string?>> GetCategoryName( string categoryUrl )
    {
        return new ServiceResponse<string?>( null, false, "Failed to validate category url!" );
    }
}