using BlazorElectronics.Client.Services.Features;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Features;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorElectronics.Client.Pages.Home;

public partial class Index : PageView
{
    [Inject] IFeaturesServiceClient FeaturesService { get; init; } = default!;
    [Inject] IJSRuntime JsRuntime { get; init; } = default!;

    bool _isCarouselInitialized = false;
    List<FeatureDto>? _features;
    List<FeatureDealDto>? _featuredDeals;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        PageIsLoaded = true;
        
        Task[] tasks = { LoadFeatures(), LoadDeals() };
        await Task.WhenAll( tasks );
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

    async Task LoadFeatures()
    {
        ServiceReply<List<FeatureDto>?> reply = await FeaturesService.GetFeatures();

        if ( reply is { Success: true, Data: not null } )
            _features = reply.Data;
        
        StateHasChanged();
    }
    async Task LoadDeals()
    {
        PaginationDto pagination = new( 20, 1 );
        ServiceReply<List<FeatureDealDto>?> reply = await FeaturesService.GetFeatureDeals( pagination );

        if ( reply is { Success: true, Data: not null } )
            _featuredDeals = reply.Data;
        
        StateHasChanged();
    }
}