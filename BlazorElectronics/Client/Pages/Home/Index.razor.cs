using BlazorElectronics.Client.Services.Products;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.Home;

public partial class Index : PageView
{
    [Inject] IProductServiceClient _productService { get; init; } = default!;

    protected override async Task OnInitializedAsync()
    {
        PageIsLoaded = true;
    }
}