using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Shared.ProductReviews;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Api.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class ReviewController : _Controller
{
    readonly IReviewService _reviewService;
    
    public ReviewController( ILogger<_Controller> logger, IReviewService reviewService )
        : base( logger )
    {
        _reviewService = reviewService;
    }
    
    [HttpPost( "for-product" )]
    public async Task<ActionResult<List<ProductReviewDto>?>> GetDetails( [FromBody] GetProductReviewsDto dto )
    {
        ServiceReply<List<ProductReviewDto>?> reply = await _reviewService.GetForProduct( dto.ProductId, dto.Rows, dto.Page );
        return GetReturnFromReply( reply );
    }
}