using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Shared.Specs;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Api.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public sealed class SpecsController : _Controller
{
    readonly ISpecLookupsService _specLookupService;

    public SpecsController( ILogger<SpecsController> logger, ISpecLookupsService specLookupService )
        : base( logger )
    {
        _specLookupService = specLookupService;
    }
    
    [HttpGet( "get" )]
    public async Task<ActionResult<LookupSpecsDto?>> GetSpecLookups()
    {
        ServiceReply<LookupSpecsDto?> reply = await _specLookupService.Get();
        return GetReturnFromReply( reply );
    }
}