using BlazorElectronics.Server.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    readonly IProductService _productService;
    readonly ICategoryService _categoryService;

    public ProductController( IProductService productService, ICategoryService categoryService )
    {
        _productService = productService;
        _categoryService = categoryService;
    }
}