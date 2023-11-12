using BlazorElectronics.Shared.Inbound.Admin.Products;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class AdminProductController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ApiReply<bool>>> AddProduct( [FromBody] AddProductRequest request )
    {
        return Ok();
    }
    [HttpPost]
    public async Task<ActionResult<ApiReply<bool>>> AddProductVariant( [FromBody] AddProductVariantRequest request )
    {
        return Ok();
    }
    
    [HttpPost]
    public async Task<ActionResult<ApiReply<bool>>> UpdateProduct( [FromBody] UpdateProductRequest request )
    {
        return Ok();
    }
    [HttpPost]
    public async Task<ActionResult<ApiReply<bool>>> UpdateProductVariant( [FromBody] UpdateProductVariantRequest request )
    {
        return Ok();
    }
    
    [HttpPost]
    public async Task<ActionResult<ApiReply<bool>>> DeleteProduct( [FromBody] DeleteProductRequest request )
    {
        return Ok();
    }
    [HttpPost]
    public async Task<ActionResult<ApiReply<bool>>> DeleteProductVariant( [FromBody] DeleteProductVariantRequest request )
    {
        return Ok();
    }
}