using BlazorElectronics.Client.Models;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Categories;

namespace BlazorElectronics.Client.Services.Admin;

public interface IAdminCategoryHelper
{
    public const string DEFAULT_ERROR_MESSAGE = "Failed to get categories: no response message!";
    
    List<CategoryViewDtoDto> Categories { get; set; }
    List<CategorySelectionOption> PrimarySelection { get; set; }

    Task<ServiceReply<bool>> Init();
    
    void ResetPrimarySelection();
    void SetPrimaryOptions( List<int> modelCategories );
    List<int> GetSelectedPrimaryOptions();
}