using BlazorElectronics.Server.Services.Categories;
using BlazorElectronics.Server.Services.Products;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers;

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