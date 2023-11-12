using BlazorElectronics.Shared.Inbound.Admin.Features;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class AdminFeaturesController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ApiReply<bool>>> AddFeature( [FromBody] AddOrUpdateFeatureRequest request )
    {
        return Ok();
    }
    [HttpPost]
    public async Task<ActionResult<ApiReply<bool>>> UpdateFeature( [FromBody] UpdateFeatureRequest request )
    {
        return Ok();
    }
    [HttpPost]
    public async Task<ActionResult<ApiReply<bool>>> DeleteFeature( [FromBody] DeleteFeatureRequest request )
    {
        return Ok();
    }

    [HttpPost]
    public async Task<ActionResult<ApiReply<bool>>> AddFeaturedDeal( [FromBody] AddOrDeleteFeaturedDealRequest request )
    {
        return Ok();
    }
    [HttpPost]
    public async Task<ActionResult<ApiReply<bool>>> DeleteFeaturedDeal( [FromBody] AddOrDeleteFeaturedDealRequest request )
    {
        return Ok();
    }
}