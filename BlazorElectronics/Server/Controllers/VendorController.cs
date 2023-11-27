using BlazorElectronics.Server.Repositories.Vendors;
using BlazorElectronics.Shared.Vendors;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class VendorController : _Controller
{
    readonly IVendorRepository _repository;
    
    public VendorController( ILogger<_Controller> logger, IVendorRepository repository )
        : base( logger )
    {
        _repository = repository;
    }
    
    [HttpGet]
    public async Task<ActionResult<ApiReply<VendorsResponse>>> GetVendors()
    {
        try
        {
            VendorsResponse? vendors = await _repository.Get();

            return vendors is not null
                ? Ok( new ApiReply<VendorsResponse>( vendors ) )
                : NotFound( NOT_FOUND_MESSAGE );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return StatusCode( StatusCodes.Status500InternalServerError, INTERNAL_SERVER_ERROR );
        }
    }
}