using BlazorElectronics.Server.Services.Vendors;
using BlazorElectronics.Shared.Vendors;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class VendorController : _Controller
{
    readonly IVendorService _vendorService;
    
    public VendorController( ILogger<_Controller> logger, IVendorService vendorService )
        : base( logger )
    {
        _vendorService = vendorService;
    }
    
    [HttpGet]
    public async Task<ActionResult<ApiReply<VendorsResponse>>> GetVendors()
    {
        return Ok( _vendorService.GetVendors() );
    }
}