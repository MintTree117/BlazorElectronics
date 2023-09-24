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
    public async Task<ActionResult<ControllerResponse<Categories_DTO>>> GetCategories()
    {
        ServiceResponse<Categories_DTO?>? response = await _categoryService.GetCategories();

        if ( response?.Data == null )
            return new ActionResult<ControllerResponse<Categories_DTO>>(
                new ControllerResponse<Categories_DTO>( null, false, "Service response is null!" ) );

        return Ok( new ControllerResponse<Categories_DTO>( response.Data, response.Success, response.Message ) );
    }
}