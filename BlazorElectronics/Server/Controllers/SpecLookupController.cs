using BlazorElectronics.Server.Repositories.SpecLookups;
using BlazorElectronics.Shared.SpecLookups;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class SpecLookupController : ControllerBase
{
    readonly ISpecLookupRepository _repository;

    public SpecLookupController( ISpecLookupRepository repository)
    {
        _repository = repository;
    }

    [HttpGet( "get-spec-lookups" )]
    public async Task<ActionResult<ApiReply<SpecLookupsResponse>>> GetSpecLookups()
    {
        return Ok( await _repository.Get() );
    }
}