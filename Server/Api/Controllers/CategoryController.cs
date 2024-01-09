using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Shared.Categories;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Api.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public sealed class CategoryController : _Controller
{
    readonly ICategoryService _categoryService;

    public CategoryController( ILogger<CategoryController> logger, ICategoryService categoryService )
        : base( logger )
    {
        _categoryService = categoryService;
    }
    
    [HttpGet( "get" )] // Returns CategoryLightDto to be reconstructed on client to full data
    public async Task<ActionResult<List<CategoryLightDto>?>> GetCategories()
    {
        ServiceReply<List<CategoryLightDto>?> reply = await _categoryService.GetDtos();
        return GetReturnFromReply( reply );
    }
}