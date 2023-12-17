using BlazorElectronics.Client.Services.Features;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Features;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.Home;

public partial class FeaturedDeals : RazorView
{
    [Inject] public IFeaturesServiceClient FeaturesService { get; set; } = default!;
    
    List<FeaturedDealDto>? _deals = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        ServiceReply<List<FeaturedDealDto>?> reply = await FeaturesService.GetFeaturedDeals();
        _deals = reply.Data;
        
        StateHasChanged();
    }
}