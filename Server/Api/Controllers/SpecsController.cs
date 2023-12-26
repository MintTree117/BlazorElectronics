using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Shared.Specs;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Api.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public sealed class SpecsController : _Controller
{
    readonly ISpecsService _specService;

    public SpecsController( ILogger<_Controller> logger, ISpecsService specService )
        : base( logger )
    {
        _specService = specService;
    }
    
    [HttpGet( "get" )]
    public async Task<ActionResult<LookupSpecsDto?>> GetSpecLookups()
    {
        ServiceReply<LookupSpecsDto?> reply = await _specService.GetSpecs();
        return GetReturnFromReply( reply );
    }
}