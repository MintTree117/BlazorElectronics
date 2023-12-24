using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Shared.Specs;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Api.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public sealed class SpecsController : _Controller
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
    public async Task<ActionResult<LookupSpecsDto?>> GetSpecLookups()
    {
        ServiceReply<List<int>?> categoryReply = await _categoryService.GetPrimaryCategoryIds();

        if ( !categoryReply.Success || categoryReply.Data is null )
            return GetReturnFromReply( categoryReply );
        
        ServiceReply<LookupSpecsDto?> reply = await _specService.GetSpecs( categoryReply.Data );
        return GetReturnFromReply( reply );
    }
}