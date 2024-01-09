using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Shared.Reviews;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Api.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public sealed class ReviewsController : _Controller
{
    readonly IReviewService _reviewService;
    
    public ReviewsController( ILogger<ReviewsController> logger, IReviewService reviewService )
        : base( logger )
    {
        _reviewService = reviewService;
    }
    
    [HttpGet( "get-for-product" )]
    public async Task<ActionResult<ProductReviewsReplyDto?>> GetForProduct( [FromQuery] ProductReviewsGetDto dto )
    {
        ServiceReply<ProductReviewsReplyDto?> reply = await _reviewService.GetForProduct( dto );
        return GetReturnFromReply( reply );
    }
    [HttpGet( "get-for-user" )]
    public async Task<ActionResult<ProductReviewsReplyDto?>> GetForUser( [FromQuery] ProductReviewsGetDto dto )
    {
        ServiceReply<ProductReviewsReplyDto?> reply = await _reviewService.GetForProduct( dto );
        return GetReturnFromReply( reply );
    }
}