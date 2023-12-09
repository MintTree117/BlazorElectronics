using BlazorElectronics.Server.Services.Categories;
using BlazorElectronics.Server.Services.Specs;
using BlazorElectronics.Shared.Specs;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class SpecsController : _Controller
{
    readonly ICategoryService _categoryService;
    readonly ISpecsService _specService;

    public SpecsController( ILogger<_Controller> logger, ICategoryService categoryService, ISpecsService specService )
        : base( logger )
    {
        _categoryService = categoryService;
        _specService = specService;
    }
    
    [HttpGet( "get" )]
    public async Task<ActionResult<SpecsResponse?>> GetSpecLookups()
    {
        ServiceReply<List<int>?> categoryReply = await _categoryService.GetPrimaryCategoryIds();

        if ( !categoryReply.Success || categoryReply.Data is null )
            return GetReturnFromReply( categoryReply );
        
        ServiceReply<SpecsResponse?> reply = await _specService.GetSpecs( categoryReply.Data );
        return GetReturnFromReply( reply );
    }
}