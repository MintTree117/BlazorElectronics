using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Shared.Features;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Api.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public sealed class FeaturesController : _Controller
{
    readonly IFeaturesService _featuresService;
    
    public FeaturesController( ILogger<_Controller> logger, IFeaturesService featuresService )
        : base( logger )
    {
        _featuresService = featuresService;
    }

    [HttpGet( "get-features" )]
    public async Task<ActionResult<List<FeatureDto>?>> GetFeaturedProducts()
    {
        ServiceReply<List<FeatureDto>?> featureReply = await _featuresService.GetFeatures();
        return GetReturnFromReply( featureReply );
    }
    [HttpGet( "get-front-page-deals" )]
    public async Task<ActionResult<List<FeatureDealDto>?>> GetFrontPageDeals()
    {
        ServiceReply<List<FeatureDealDto>?> featureReply = await _featuresService.GetDeals( 20, 1 );
        return GetReturnFromReply( featureReply );
    }
    [HttpGet( "get-deals" )]
    public async Task<ActionResult<List<FeatureDealDto>?>> GetFeaturedDeals( int rows, int page )
    {
        ServiceReply<List<FeatureDealDto>?> featureReply = await _featuresService.GetDeals( rows, page );
        return GetReturnFromReply( featureReply );
    }
}