using BlazorElectronics.Shared.DataTransferObjects.Categories;

namespace BlazorElectronics.Client.Services.Categories;

public interface ICategoryService
{
    public string ControllerMessage { get; set; }
    List<Category_DTO> PrimaryCategories { get; set; }
    List<CategorySub_DTO> SubCategories { get; set; }

    Task GetCategories();
}