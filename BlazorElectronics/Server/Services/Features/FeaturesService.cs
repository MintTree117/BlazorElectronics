using BlazorElectronics.Server.Models.Features;
using BlazorElectronics.Server.Repositories.Features;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Outbound.Features;

namespace BlazorElectronics.Server.Services.Features;

public class FeaturesService : IFeaturesService
{
    readonly IFeaturesRepository _featuresRepository;
    readonly IFeaturesCache _cache;

    public FeaturesService( IFeaturesRepository featuresRepository, IFeaturesCache cache )
    {
        _featuresRepository = featuresRepository;
        _cache = cache;
    }
    public async Task<ServiceResponse<FeaturedProducts_DTO?>> GetFeaturedProducts()
    {
        FeaturedProducts_DTO? dto = await _cache.GetFeaturedProducts();

        if ( dto != null )
            return new ServiceResponse<FeaturedProducts_DTO?>( dto, true, "Success. Retrieved FeaturedProducts_DTO from cache." );

        IEnumerable<FeaturedProduct>? models = await _featuresRepository.GetFeaturedProducts();

        if ( models == null )
            return new ServiceResponse<FeaturedProducts_DTO?>( null, false, "Failed to retrieve FeaturedProducts_DTO from cache, and ProductDetails from repository!" );

        dto = await MapFeaturedProductsToDto( models );
        await _cache.CacheFeaturedProducts( dto );

        return new ServiceResponse<FeaturedProducts_DTO?>( dto, true, "Successfully retrieved FeaturedProducts_DTO from repository, mapped to DTO, and cached." );
    }
    public async Task<ServiceResponse<FeaturedDeals_DTO?>> GetFeaturedDeals()
    {
        FeaturedDeals_DTO? dto = await _cache.GetFeaturedDeals();

        if ( dto != null )
            return new ServiceResponse<FeaturedDeals_DTO?>( dto, true, "Success. Retrieved Top Deals from cache." );

        IEnumerable<FeaturedDeal>? models = await _featuresRepository.GetFeaturedDeals();

        if ( models == null )
            return new ServiceResponse<FeaturedDeals_DTO?>( null, false, "Failed to retrieve Top Deals from cache, and ProductDetails from repository!" );

        dto = await MapFeaturedDealsToDto( models );
        await _cache.CacheFeaturedDeals( dto );

        return new ServiceResponse<FeaturedDeals_DTO?>( dto, true, "Successfully retrieved Top Deals from repository, mapped to DTO, and cached." );

    }
    static async Task<FeaturedProducts_DTO> MapFeaturedProductsToDto( IEnumerable<FeaturedProduct> models )
    {
        var dtos = new List<FeaturedProduct_DTO>();

        await Task.Run( () =>
        {
            foreach ( FeaturedProduct f in models )
            {
                dtos.Add( new FeaturedProduct_DTO {
                    ProductId = f.ProductId,
                    ImageUrl = f.FeatureImageUrl
                } );
            }
        } );

        return new FeaturedProducts_DTO {
            FeaturedProducts = dtos
        };
    }
    static async Task<FeaturedDeals_DTO> MapFeaturedDealsToDto( IEnumerable<FeaturedDeal> models )
    {
        var deals = new List<FeaturedDeal_DTO>();

        await Task.Run( () =>
        {
            foreach ( FeaturedDeal f in models )
            {
                deals.Add( new FeaturedDeal_DTO {
                    ProductId = f.ProductId,
                    VariantId = f.VariantId,
                    ProductTitle = f.ProductTitle,
                    ProductRating = f.ProductRating,
                    ProductThumbnail = f.ProductThumbnail,
                    OriginalPrice = f.OriginalPrice,
                    SalePrice = f.SalePrice
                } );
            }
        } );
        
        return new FeaturedDeals_DTO {
            Deals = deals
        };
    }
}