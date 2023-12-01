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
    
    [HttpGet("get-categories")]
    public async Task<ActionResult<CategoriesResponse>> GetCategories()
    {
        ServiceReply<CategoriesResponse?> reply = await _categoryService.GetCategoriesResponse();
        return GetReturnFromApi( reply );
    }
}