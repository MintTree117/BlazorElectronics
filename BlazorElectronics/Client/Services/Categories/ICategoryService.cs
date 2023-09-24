using BlazorElectronics.Shared.DataTransferObjects.Categories;

namespace BlazorElectronics.Client.Services.Categories;

public interface ICategoryService
{
    public string ControllerMessage { get; set; }
    List<Category_DTO> Categories { get; set; }

    Task GetCategories();
}