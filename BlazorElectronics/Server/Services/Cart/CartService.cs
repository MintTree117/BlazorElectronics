using BlazorElectronics.Server.Models.Cart;
using BlazorElectronics.Server.Models.Products;
using BlazorElectronics.Server.Repositories.Cart;
using BlazorElectronics.Shared.Inbound.Cart;
using BlazorElectronics.Shared.Mutual;

namespace BlazorElectronics.Server.Services.Cart;

public class CartService : ICartService
{
    readonly ICartRepository _cartRepository;

    public CartService( ICartRepository cartRepository )
    {
        _cartRepository = cartRepository;
    }
    
    public async Task<ApiReply<Cart_DTO?>> PostCartItems( int userId, List<CartItemId_DTO> cartItemsDtos )
    {
        List<CartItem> models = MapItemIdsToModels( cartItemsDtos, userId );
        
        try
        {
            await _cartRepository.AddCartItems( models );
        }
        catch ( ServiceException e )
        {
            return new ApiReply<Cart_DTO?>( e.Message );
        }
        
        IEnumerable<CartItem>? allItems;
        
        try
        {
            allItems = await _cartRepository.GetCartItems( userId );
        }
        catch ( ServiceException e )
        {
            return new ApiReply<Cart_DTO?>( e.Message );
        }
        
        GetProductIdsFromModels( allItems!, out List<int> productIds, out List<int> variantIds );
        IEnumerable<Product>? cartProducts;
        
        try
        {
            cartProducts = await _cartRepository.GetCartProducts( productIds, variantIds );
        }
        catch ( ServiceException e )
        {
            return new ApiReply<Cart_DTO?>( e.Message );
        }

        Cart_DTO cartDto = await MapProductsToCart( cartProducts!, cartItemsDtos );
        return new ApiReply<Cart_DTO?>( cartDto, true, "Successfully inserted Cart Items to database, and retrieved Cart Products from database." );
    }
    public async Task<ApiReply<bool>> AddToCart( int userId, CartItemId_DTO cartItem )
    {
        CartItem modelItem = MapItemIdsToModel( cartItem, userId );

        try
        {
            await _cartRepository.AddCartItem( modelItem );
        }
        catch ( ServiceException e )
        {
            return new ApiReply<bool>( e.Message );
        }

        return new ApiReply<bool>( true, true, "Successfully inserted Cart Item into database." );
    }
    public async Task<ApiReply<bool>> UpdateQuantity( int userId, CartItemId_DTO cartItem )
    {
        CartItem modelItem = MapItemIdsToModel( cartItem, userId );

        try
        {
            await _cartRepository.UpdateCartItemQuantity( modelItem );
        }
        catch ( ServiceException e )
        {
            return new ApiReply<bool>( e.Message );
        }

        return new ApiReply<bool>( true, true, "Successfully updated Cart Item Quantity in database." );
    }
    public async Task<ApiReply<bool>> RemoveFromCart( int userId, CartItemId_DTO cartItem )
    {
        CartItem modelItem = MapItemIdsToModel( cartItem, userId );

        try
        {
            await _cartRepository.RemoveCartItem( modelItem );
        }
        catch ( ServiceException e )
        {
            return new ApiReply<bool>( e.Message );
        }

        return new ApiReply<bool>( true, true, "Successfully removed Cart Item from database." );
    }
    public async Task<ApiReply<int>> CountCartItems( int userId )
    {
        int? count = null;

        try
        {
            count = await _cartRepository.CountCartItems( userId );
            if ( count is null or <= 0 )
                return new ApiReply<int>( $"No cart items found!" );
        }
        catch ( ServiceException e )
        {
            return new ApiReply<int>( e.Message );
        }

        return new ApiReply<int>( count.Value, true, $"Found cart items for user {userId}" );
    }
    public async Task<ApiReply<Cart_DTO?>> GetCartProducts( int userId )
    {
        IEnumerable<CartItem>? cartItems = null;

        try
        {
            await _cartRepository.GetCartItems( userId );
        }
        catch ( ServiceException e )
        {
            return new ApiReply<Cart_DTO?>( e.Message );
        }
        
        List<CartItem> itemsList = cartItems!.ToList();
        GetProductIdsFromModels( itemsList, out List<int> productIds, out List<int> variantIds );

        IEnumerable<Product>? cartProducts = null;

        try
        {
            await _cartRepository.GetCartProducts( productIds, variantIds );
        }
        catch ( ServiceException e )
        {
            return new ApiReply<Cart_DTO?>( e.Message );
        }

        Cart_DTO cartDto = await MapProductsToCart( cartProducts!, itemsList.ToList() );
        return new ApiReply<Cart_DTO?>( cartDto, true, "Successfully retrieved Cart Products from database." );
    }
    
    static void GetProductIdsFromModels( IEnumerable<CartItem> models, out List<int> productIds, out List<int> variantIds )
    {
        productIds = new List<int>();
        variantIds = new List<int>();

        foreach ( CartItem item in models )
        {
            productIds.Add( item.ProductId );
            variantIds.Add( item.VariantId );
        }
    }
    static List<CartItem> MapItemIdsToModels( List<CartItemId_DTO> cartItems, int userId )
    {
        var models = new List<CartItem>();

        foreach ( CartItemId_DTO dto in cartItems )
            models.Add( MapItemIdsToModel( dto, userId ) );

        return models;
    }
    static CartItem MapItemIdsToModel( CartItemId_DTO dto, int userId )
    {
        return new CartItem {
            UserId = userId,
            ProductId = dto.ProductId,
            VariantId = dto.VariantId,
            Quantity = dto.Quantity
        };
    }
    static async Task<Cart_DTO> MapProductsToCart( IEnumerable<Product> products, List<CartItemId_DTO> dtos )
    {
        var cartDto = new Cart_DTO();

        await Task.Run( () =>
        {
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
                    SalePrice = variantData.VariantPriceSale,
                    Quantity = dtos.Find( x => x.ProductId == p.ProductId && x.VariantId == variantData.VariantId )!.Quantity
                } );
            }
        } );

        return cartDto;
    }
    static async Task<Cart_DTO> MapProductsToCart( IEnumerable<Product> products, List<CartItem> items )
    {
        var cartDto = new Cart_DTO();

        await Task.Run( () =>
        {
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
                    SalePrice = variantData.VariantPriceSale,
                    Quantity = items.Find( x => x.ProductId == p.ProductId && x.VariantId == variantData.VariantId )!.Quantity
                } );
            }
        } );

        return cartDto;
    }
}