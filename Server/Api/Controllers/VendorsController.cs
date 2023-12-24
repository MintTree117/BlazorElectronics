using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Shared.Vendors;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Api.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public sealed class VendorsController : _Controller
{
    readonly IVendorService _vendorService;
    
    public VendorsController( ILogger<_Controller> logger, IVendorService vendorService )
        : base( logger )
    {
        _vendorService = vendorService;
    }
    
    [HttpGet( "get" )]
    public async Task<ActionResult<VendorsDto?>> GetVendors()
    {
        ServiceReply<VendorsDto?> reply = await _vendorService.GetVendors();
        return GetReturnFromReply( reply );
    }
}