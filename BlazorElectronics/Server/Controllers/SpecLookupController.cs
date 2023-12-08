using BlazorElectronics.Server.Repositories.Specs;
using BlazorElectronics.Shared.SpecLookups;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class SpecLookupController : ControllerBase
{
    readonly ISpecRepository _repository;

    public SpecLookupController( ISpecRepository repository)
    {
        _repository = repository;
    }

    [HttpGet( "get-spec-lookups" )]
    public async Task<ActionResult<ServiceReply<SpecsResponse>>> GetSpecLookups()
    {
        return Ok( await _repository.Get() );
    }
}