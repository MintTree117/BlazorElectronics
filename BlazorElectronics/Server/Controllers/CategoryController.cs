using BlazorElectronics.Server.Services.Categories;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DataTransferObjects.Categories;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class CategoryController : ControllerBase
{
    readonly ICategoryService _categoryService;

    public CategoryController( ICategoryService categoryService )
    {
        _categoryService = categoryService;
    }
    
    [HttpGet("categories")]
    public async Task<ActionResult<ServiceResponse<Categories_DTO>>> GetCategories()
    {
        ServiceResponse<Categories_DTO?> response = await _categoryService.GetCategoriesDto();
        return Ok( response );
    }
}