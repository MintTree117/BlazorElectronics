namespace BlazorElectronics.Shared.DataTransferObjects.Categories;

public sealed class CategoryLists_DTO
{
    public List<Category_DTO> PrimaryCategories = new();
    public List<CategorySub_DTO> SubCategories = new();
}