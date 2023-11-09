using BlazorElectronics.Server.Services.Categories;
using BlazorElectronics.Server.Services.Specs;
using BlazorElectronics.Shared.Mutual;
using BlazorElectronics.Shared.Outbound.Specs;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class SpecLookupController : ControllerBase
{
    readonly ISpecLookupService _specLookupService;
    readonly ICategoryService _categoryService;

    public SpecLookupController( ISpecLookupService specLookupService, ICategoryService categoryService )
    {
        _specLookupService = specLookupService;
        _categoryService = categoryService;
    }

    [HttpGet( "global" )]
    public async Task<ActionResult<Reply<SpecFiltersResponse>>> GetSpecLookupsGlobal()
    {
        return Ok( await _specLookupService.GetSpecFiltersResponse() );
    }
    [HttpGet( "category/{primaryCategoryUrl}" )]
    public async Task<ActionResult<Reply<SpecFiltersResponse>>> GetSpecLookupsCategory( string primaryCategoryUrl )
    {
        Reply<CategoryIdMap?> categoryResponse = await _categoryService.GetCategoryIdMapFromUrl( primaryCategoryUrl );

        if ( !categoryResponse.Success )
            return BadRequest( new Reply<SpecFiltersResponse>( categoryResponse.Message ) );

        return Ok( await _specLookupService.GetSpecFiltersResponse( categoryResponse.Data!.CategoryId ) );
    }
}