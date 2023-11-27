using BlazorElectronics.Shared.Vendors;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class VendorController : ControllerBase
{
    [HttpGet]
    public async Task<ApiReply<VendorsResponse>> GetVendors()
    {
        return null;
    }
}