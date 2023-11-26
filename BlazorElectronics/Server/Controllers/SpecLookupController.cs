using BlazorElectronics.Server.Services.SpecLookups;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.SpecLookups;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class SpecLookupController : ControllerBase
{
    readonly ISpecLookupService _specLookupService;

    public SpecLookupController( ISpecLookupService specLookupService)
    {
        _specLookupService = specLookupService;
    }

    [HttpGet( "get-spec-lookups-global" )]
    public async Task<ActionResult<ApiReply<List<SpecLookupResponse>>>> GetSpecLookups()
    {
        return Ok( await _specLookupService.GetSpecLookups() );
    }
    [HttpGet( "get-spec-lookups-category")]
    public async Task<ActionResult<ApiReply<List<SpecLookupResponse>>>> GetSpecLookups( PrimaryCategory category )
    {
        return Ok( await _specLookupService.GetSpecLookups( category ) );
    }
}