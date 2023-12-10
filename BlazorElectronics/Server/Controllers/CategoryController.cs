using BlazorElectronics.Server.Services.Categories;
using BlazorElectronics.Shared.Categories;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class CategoryController : _Controller
{
    readonly ICategoryService _categoryService;

    public CategoryController( ILogger<_Controller> logger, ICategoryService categoryService )
        : base( logger )
    {
        _categoryService = categoryService;
    }
    
    [HttpGet("get")]
    public async Task<ActionResult<List<CategoryResponse>?>> GetCategories()
    {
        ServiceReply<List<CategoryResponse>?> reply = await _categoryService.GetCategoryResponse();
        return GetReturnFromReply( reply );
    }
}