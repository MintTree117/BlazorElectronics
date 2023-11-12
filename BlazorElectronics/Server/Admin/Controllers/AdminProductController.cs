using BlazorElectronics.Shared.Admin;
using BlazorElectronics.Shared.Admin.Products;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Admin.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class AdminProductController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ApiReply<bool>>> AddProduct( [FromBody] AdminRequest<AddUpdateProductDto> request )
    {
        return Ok();
    }
    [HttpPost]
    public async Task<ActionResult<ApiReply<bool>>> UpdateProduct( [FromBody] AdminRequest<AddUpdateProductDto> request )
    {
        return Ok();
    }
    [HttpPost]
    public async Task<ActionResult<ApiReply<bool>>> DeleteProduct( [FromBody] AdminRequest<DeleteProductDto> request )
    {
        return Ok();
    }
}