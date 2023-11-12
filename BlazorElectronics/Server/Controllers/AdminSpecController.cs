using BlazorElectronics.Server.Repositories.Specs;
using BlazorElectronics.Shared.Inbound.Admin;
using BlazorElectronics.Shared.Inbound.Admin.SpecLookups;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class AdminSpecController : ControllerBase
{
    IAdminSpecLookupRepository _repository;
    
    public AdminSpecController( IAdminSpecLookupRepository repository )
    {
        _repository = repository;
    }

    [HttpPost($"{AdminControllerRoutes.AddSpecSingle}")]
    public async Task<ActionResult<ApiReply<bool>>> AddSpecLookupSingle( [FromBody] AddSpecSingleRequest request )
    {
        return Ok();
    }
    [HttpPost( $"{AdminControllerRoutes.UpdateSpecSingle}" )]
    public async Task<ActionResult<ApiReply<bool>>> UpdateSpecLookupSingle( [FromBody] UpdateSpecSingleRequest request )
    {
        return Ok();
    }
    [HttpPost( $"{AdminControllerRoutes.DeleteSpecSingle}" )]
    public async Task<ActionResult<ApiReply<bool>>> DeleteSpecLookupSingle( [FromBody] DeleteSpecSingleRequest request )
    {
        return Ok();
    }

    [HttpPost( $"{AdminControllerRoutes.AddSpecMulti}" )]
    public async Task<ActionResult<ApiReply<bool>>> AddSpecLookupMulti( [FromBody] AddSpecMultiTableRequest tableRequest )
    {
        return Ok();
    }
    [HttpPost( $"{AdminControllerRoutes.UpdateSpecMuti}" )]
    public async Task<ActionResult<ApiReply<bool>>> UpdateSpecLookupMulti( [FromBody] AddSpecMultiTableRequest tableRequest )
    {
        return Ok();
    }
    [HttpPost( $"{AdminControllerRoutes.DeleteSpecMulti}" )]
    public async Task<ActionResult<ApiReply<bool>>> DeleteSpecLookupMulti( [FromBody] DeleteSpecMultiTableRequest tableRequest )
    {
        return Ok();
    }
}