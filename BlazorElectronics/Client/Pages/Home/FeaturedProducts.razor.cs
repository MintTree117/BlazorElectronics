using BlazorElectronics.Client.Services.Features;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Features;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorElectronics.Client.Pages.Home;

public partial class FeaturedProducts : RazorView
{
    [Inject] IFeaturesServiceClient FeaturesService { get; init; } = default!;
    [Inject] IJSRuntime JsRuntime { get; init; } = default!;
    
    bool _isCarouselInitialized = false;
    List<FeaturedProductDto>? _featuredProducts;

    protected override async Task OnInitializedAsync()
    {
        ServiceReply<List<FeaturedProductDto>?> reply = await FeaturesService.GetFeaturedProducts();

        if ( reply.Success )
            _featuredProducts = reply.Data;
    }
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        try
        {
            if ( !_isCarouselInitialized )
            {
                await JsRuntime.InvokeVoidAsync( "startCarousel" );
                _isCarouselInitialized = true;
            }
        }
        catch ( Exception e )
        {
            Logger.LogError( e.Message, e );
        }
    }
}