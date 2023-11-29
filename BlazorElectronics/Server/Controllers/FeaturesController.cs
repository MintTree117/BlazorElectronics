using BlazorElectronics.Server.Services.Features;
using BlazorElectronics.Shared.Features;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class FeaturesController : _Controller
{
    readonly IFeaturesService _featuresService;
    
    public FeaturesController( ILogger<_Controller> logger, IFeaturesService featuresService )
        : base( logger )
    {
        _featuresService = featuresService;
    }

    [HttpGet( "get-features" )]
    public async Task<ActionResult<FeaturesResponse>> GetFeaturedProducts()
    {
        ApiReply<FeaturesResponse?> featureReply = await _featuresService.GetFeatures();
        return GetReturnFromApi( featureReply );
    }
}