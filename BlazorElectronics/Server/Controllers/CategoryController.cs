using BlazorElectronics.Server.Services;
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
    public async Task<ActionResult<ControllerResponse<CategoryLists_DTO>>> GetCategories()
    {
        ServiceResponse<CategoryLists_DTO?>? response = await _categoryService.GetCategories();

        if ( response == null )
            return new ActionResult<ControllerResponse<CategoryLists_DTO>>(
                new ControllerResponse<CategoryLists_DTO>( null, false, "Service response is null!" ) );

        return Ok( new ControllerResponse<CategoryLists_DTO>( response.Data, response.Success, response.Message ) );
    }
}