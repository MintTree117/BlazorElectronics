using BlazorElectronics.Client.Services.Products;
using BlazorElectronics.Client.Services.Specs;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.ProductReviews;
using BlazorElectronics.Shared.Products;
using BlazorElectronics.Shared.Specs;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.Products;

public partial class ProductDetails : PageView
{
    [Inject] public IProductServiceClient ProductService { get; init; } = default!;
    [Inject] public IProductReviewServiceClient ReviewService { get; set; } = default!;
    [Inject] public ISpecServiceClient LookupService { get; set; } = default!;

    [Parameter] public string ProductId { get; set; } = string.Empty;
    
    ProductDto? _product;
    readonly Dictionary<string, string> _lookupSpecsAggregated = new();
    List<ProductReviewDto> _reviews = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if ( !int.TryParse( ProductId, out int id ) )
        {
            SetViewMessage( false, "Invalid Url!" );
            return;
        }

        ServiceReply<ProductDto?> productReply = await ProductService.GetProductDetails( id );
        PageIsLoaded = true;
        
        if ( !productReply.Success || productReply.Data is null )
        {
            InvokeAlert( AlertType.Danger, $"Failed to fetch product details! {productReply.ErrorType} : {productReply.Message}" );
            PageIsLoaded = true;
            return;
        }

        _product = productReply.Data;

        ServiceReply<SpecsResponse?> lookupsReply = await LookupService.GetSpecLookups();

        if ( lookupsReply is { Success: true, Data: not null } )
            AggregateLookupSpecs( lookupsReply.Data );
    }
    public override bool PageIsReady()
    {
        return base.PageIsReady() && _product is not null;
    }

    void AggregateLookupSpecs( SpecsResponse lookups )
    {
        foreach ( KeyValuePair<int, List<int>> kvp in _product.LookupSpecs )
        {
            if ( !lookups.SpecsById.TryGetValue( kvp.Key, out Spec? lookup ) )
                continue;

            List<string> valuesToAggregate = ( 
                from valueId in kvp.Value 
                where valueId >= 0 && valueId < lookup.Values.Count 
                select lookup.Values[ valueId ] )
                .ToList();

            _lookupSpecsAggregated.TryAdd( lookup.SpecName, string.Join( ", ", valuesToAggregate ) );
        }
    }
    async Task GetReviews( int rows, int page )
    {
        GetProductReviewsDto dto = new()
        {
            ProductId = _product.Id,
            Rows = rows,
            Page = page
        };

        ServiceReply<List<ProductReviewDto>?> reply = await ReviewService.GetForProduct( dto );
    }
}