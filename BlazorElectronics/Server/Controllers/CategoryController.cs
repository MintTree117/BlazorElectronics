using BlazorElectronics.Server.Services.Categories;
using BlazorElectronics.Shared.Categories;
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
    public async Task<ActionResult<ApiReply<CategoriesResponse>>> GetCategories()
    {
        ApiReply<CategoriesResponse?> response = await _categoryService.GetCategoriesResponse();
        return Ok( response );
    }
}