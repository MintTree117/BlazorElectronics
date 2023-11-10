using BlazorElectronics.Server.Services.Categories;
using BlazorElectronics.Shared.Mutual;
using BlazorElectronics.Shared.Outbound.Categories;
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
    public async Task<ActionResult<Reply<CategoriesResponse>>> GetCategories()
    {
        Reply<CategoriesResponse?> response = await _categoryService.GetCategories();
        return Ok( response );
    }
    [HttpGet( "main-descriptions" )]
    public async Task<ActionResult<Reply<IReadOnlyList<string?>>>> GetMainDescriptions()
    {
        Reply<IReadOnlyList<string>?> response = await _categoryService.GetMainDescriptions();
        return Ok( response );
    }
    [HttpPost( "get-description" )]
    public async Task<ActionResult<Reply<string?>>> GetDescription( [FromBody] CategoryIdMap idMap )
    {
        Reply<string?> reply = await _categoryService.GetDescription( idMap );
        return Ok( reply );
    }
}