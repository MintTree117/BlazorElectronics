using BlazorElectronics.Server.Services.Specs;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DataTransferObjects.Specs;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class SpecFilterController : ControllerBase
{
    readonly ISpecService _specService;

    public SpecFilterController( ISpecService specService )
    {
        _specService = specService;
    }
    
    [HttpGet("SpecFilters")]
    public async Task<ActionResult<ServiceResponse<SpecFilters_DTO>>> GetSpecFilters( string categoryUrl )
    {
        ServiceResponse<SpecFilters_DTO> response = await _specService.GetSpecFilters( categoryUrl );
        return Ok( response );
    }
}