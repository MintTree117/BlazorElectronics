using BlazorElectronics.Client.Pages;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client;

public abstract class RazorView : ComponentBase
{
    [Inject] protected ILogger<PageView> Logger { get; init; } = default!;
    [Inject] protected NavigationManager NavManager { get; init; } = default!;
}