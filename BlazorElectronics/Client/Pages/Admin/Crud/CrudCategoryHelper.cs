using BlazorElectronics.Client.Services.Admin;
using BlazorElectronics.Shared.Categories;

namespace BlazorElectronics.Client.Pages.Admin.Crud;

public sealed class CrudCategoryHelper
{
    readonly IAdminCrudService<CategoryView, CategoryEdit> _categoryService;
    
    public CrudCategoryHelper( IAdminCrudService<CategoryView, CategoryEdit> categoryService )
    {
        _categoryService = categoryService;
    }
}