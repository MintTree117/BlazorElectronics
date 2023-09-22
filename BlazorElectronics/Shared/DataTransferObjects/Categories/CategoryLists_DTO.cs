namespace BlazorElectronics.Shared.DataTransferObjects.Categories;

public sealed class CategoryLists_DTO
{
    // REMEMBER TO MAAKE MEMBERS PROPERTIES!!!!!!!!!!!
    public List<Category_DTO> PrimaryCategories { get; set; } = new();
    public List<CategorySub_DTO> SubCategories { get; set; } = new();
}