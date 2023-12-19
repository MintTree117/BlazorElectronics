using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Shared.Reviews;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Api.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public sealed class ReviewsController : _Controller
{
    readonly IReviewService _reviewService;
    
    public ReviewsController( ILogger<_Controller> logger, IReviewService reviewService )
        : base( logger )
    {
        _reviewService = reviewService;
    }
    
    [HttpPost( "for-product" )]
    public async Task<ActionResult<ProductReviewsReplyDto?>> GetDetails( [FromBody] ProductReviewsGetDto dto )
    {
        ServiceReply<ProductReviewsReplyDto?> reply = await _reviewService.GetForProduct( dto );
        return GetReturnFromReply( reply );
    }
}