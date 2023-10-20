using BlazorElectronics.Server.Models.Products;
using BlazorElectronics.Server.Repositories.Products;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DtosOutbound.Products;
using BlazorElectronics.Shared.Inbound.Cart;
using BlazorElectronics.Shared.Mutual;

namespace BlazorElectronics.Server.Services.Cart;

public class CartService : ICartService
{
    readonly IProductCartRepository _cartRepository;
    
    public CartService( IProductCartRepository cartRepository )
    {
        _cartRepository = cartRepository;
    }

    public async Task<ServiceResponse<Cart_DTO?>> GetOrUpdateCartItems( CartItemIds clientItems )
    {
        var productIds = new List<int>();
        var variantIds = new List<int>();

        foreach ( CartItemId id in clientItems.Items )
        {
            productIds.Add( id.ProductId );
            variantIds.Add( id.VariantId );
        }

        IEnumerable<Product>? products = await _cartRepository.GetCartItems( productIds, variantIds );

        if ( products == null )
            return new ServiceResponse<Cart_DTO?>( null, false, "Failed to retrieve Cart Products from repository!" );

        var cartDto = new Cart_DTO();
        
        foreach ( Product p in products )
        {
            if ( p.ProductVariants.Count <= 0 )
                continue;

            ProductVariant variantData = p.ProductVariants[ 0 ];

            cartDto.Items.Add( new CartItem_DTO {
                ProductId = p.ProductId,
                ProductTitle = p.ProductTitle,
                ProductThumbnail = p.ProductThumbnail,
                VariantId = variantData.VariantId,
                VariantName = variantData.VariantName,
                MainPrice = variantData.VariantPriceMain,
                SalePrice = variantData.VariantPriceSale
            } );
        }

        return new ServiceResponse<Cart_DTO?>( cartDto, true, "Successfully retrieved cart items from repository." );
    }
}