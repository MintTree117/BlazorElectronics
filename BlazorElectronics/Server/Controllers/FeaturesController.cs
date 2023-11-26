using BlazorElectronics.Server.Services.Features;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Features;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class FeaturesController : ControllerBase
{
    readonly IFeaturesService _featuresService;
    
    public FeaturesController( IFeaturesService featuresService )
    {
        _featuresService = featuresService;
    }

    [HttpGet( "products" )]
    public async Task<ActionResult<ApiReply<FeaturedProductsResponse>>> GetFeaturedProducts()
    {
        //Reply<FeaturedProducts_DTO?> response = await _featuresService.GetFeaturedProducts();
        //return Ok( response );
        return Ok();
    }
    [HttpGet( "deals" )]
    public async Task<ActionResult<ApiReply<FeaturedDealsResponse>>> GetTopDeals()
    {
        //Reply<FeaturedDeals_DTO?> response = await _featuresService.GetFeaturedDeals();
        //return Ok( response );
        return Ok();
    }
}