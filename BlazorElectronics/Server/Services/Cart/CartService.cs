using BlazorElectronics.Server.Models.Cart;
using BlazorElectronics.Server.Models.Products;
using BlazorElectronics.Server.Repositories.Cart;
using BlazorElectronics.Shared.Cart;

namespace BlazorElectronics.Server.Services.Cart;

public class CartService : ApiService, ICartService
{
    readonly ICartRepository _cartRepository;

    public CartService( ILogger<ApiService> logger, ICartRepository cartRepository )
        : base( logger )
    {
        _cartRepository = cartRepository;
    }
    
    public async Task<ApiReply<CartResponse?>> PostCartItems( int userId, List<CartItemIdsDto> cartItemsDtos )
    {
        List<CartItem> models = MapItemIdsToModels( cartItemsDtos, userId );
        
        try
        {
            await _cartRepository.AddCartItems( models );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ApiReply<CartResponse?>( ServiceErrorType.ServerError, e.Message );
        }
        
        IEnumerable<CartItem>? allItems;
        
        try
        {
            allItems = await _cartRepository.GetCartItems( userId );
        }
        catch ( ServiceException e )
        {
            return new ApiReply<CartResponse?>( ServiceErrorType.ServerError, e.Message );
        }
        
        GetProductIdsFromModels( allItems!, out List<int> productIds, out List<int> variantIds );
        IEnumerable<CartProductModel>? cartProducts;
        
        try
        {
            cartProducts = await _cartRepository.GetCartProducts( productIds, variantIds );
        }
        catch ( ServiceException e )
        {
            return new ApiReply<CartResponse?>( ServiceErrorType.ServerError, e.Message );
        }

        CartResponse cartResponse = await MapProductsToCart( cartProducts!, cartItemsDtos );
        return new ApiReply<CartResponse?>( cartResponse );
    }
    public async Task<ApiReply<bool>> AddToCart( int userId, CartItemIdsDto cartItem )
    {
        CartItem modelItem = MapItemIdsToModel( cartItem, userId );

        try
        {
            await _cartRepository.AddCartItem( modelItem );
        }
        catch ( ServiceException e )
        {
            return new ApiReply<bool>( ServiceErrorType.ServerError, e.Message );
        }

        return new ApiReply<bool>( true );
    }
    public async Task<ApiReply<bool>> UpdateQuantity( int userId, CartItemIdsDto cartItem )
    {
        CartItem modelItem = MapItemIdsToModel( cartItem, userId );

        try
        {
            await _cartRepository.UpdateCartItemQuantity( modelItem );
        }
        catch ( ServiceException e )
        {
            return new ApiReply<bool>( ServiceErrorType.ServerError, e.Message );
        }

        return new ApiReply<bool>( true );
    }
    public async Task<ApiReply<bool>> RemoveFromCart( int userId, CartItemIdsDto cartItem )
    {
        CartItem modelItem = MapItemIdsToModel( cartItem, userId );

        try
        {
            await _cartRepository.RemoveCartItem( modelItem );
        }
        catch ( ServiceException e )
        {
            return new ApiReply<bool>( ServiceErrorType.ServerError, e.Message );
        }

        return new ApiReply<bool>( true );
    }
    public async Task<ApiReply<int>> CountCartItems( int userId )
    {
        int? count = null;

        try
        {
            count = await _cartRepository.CountCartItems( userId );
            if ( count is null or <= 0 )
                return new ApiReply<int>( ServiceErrorType.NotFound, $"No cart items found!" );
        }
        catch ( ServiceException e )
        {
            return new ApiReply<int>( ServiceErrorType.ServerError, e.Message );
        }

        return new ApiReply<int>( count.Value );
    }
    public async Task<ApiReply<CartResponse?>> GetCartProducts( int userId )
    {
        IEnumerable<CartItem>? cartItems = null;

        try
        {
            await _cartRepository.GetCartItems( userId );
        }
        catch ( ServiceException e )
        {
            return new ApiReply<CartResponse?>( ServiceErrorType.ServerError, e.Message );
        }
        
        List<CartItem> itemsList = cartItems!.ToList();
        GetProductIdsFromModels( itemsList, out List<int> productIds, out List<int> variantIds );

        IEnumerable<CartProductModel>? cartProducts = null;

        try
        {
            await _cartRepository.GetCartProducts( productIds, variantIds );
        }
        catch ( ServiceException e )
        {
            return new ApiReply<CartResponse?>( ServiceErrorType.ServerError, e.Message );
        }

        CartResponse cartResponse = await MapProductsToCart( cartProducts!, itemsList.ToList() );
        return new ApiReply<CartResponse?>( cartResponse );
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
    static List<CartItem> MapItemIdsToModels( List<CartItemIdsDto> cartItems, int userId )
    {
        var models = new List<CartItem>();

        foreach ( CartItemIdsDto dto in cartItems )
            models.Add( MapItemIdsToModel( dto, userId ) );

        return models;
    }
    static CartItem MapItemIdsToModel( CartItemIdsDto dto, int userId )
    {
        return new CartItem {
            UserId = userId,
            ProductId = dto.ProductId,
            VariantId = dto.VariantId,
            Quantity = dto.Quantity
        };
    }
    static async Task<CartResponse> MapProductsToCart( IEnumerable<CartProductModel> products, List<CartItemIdsDto> dtos )
    {
        var cartDto = new CartResponse();

        await Task.Run( () =>
        {
            foreach ( CartProductModel p in products )
            {
                /*if ( p.ProductVariants.Count <= 0 )
                    continue;

                ProductVariantModel variantModelData = p.ProductVariants[ 0 ];

                cartDto.Items.Add( new CartProductResponse {
                    ProductId = p.ProductId,
                    ProductTitle = p.ProductTitle,
                    ProductThumbnail = p.ProductThumbnail,
                    VariantId = variantModelData.VariantId,
                    VariantName = variantModelData.VariantName,
                    MainPrice = variantModelData.VariantPriceMain,
                    SalePrice = variantModelData.VariantPriceSale,
                    Quantity = dtos.Find( x => x.ProductId == p.ProductId && x.VariantId == variantModelData.VariantId )!.Quantity
                } );*/
            }
        } );

        return cartDto;
    }
    static async Task<CartResponse> MapProductsToCart( IEnumerable<CartProductModel> products, List<CartItem> items )
    {
        var cartDto = new CartResponse();

        await Task.Run( () =>
        {
            foreach ( CartProductModel p in products )
            {
                /*if ( p.ProductVariants.Count <= 0 )
                    continue;

                ProductVariantModel variantModelData = p.ProductVariants[ 0 ];

                cartDto.Items.Add( new CartProductResponse {
                    ProductId = p.ProductId,
                    ProductTitle = p.ProductTitle,
                    ProductThumbnail = p.ProductThumbnail,
                    VariantId = variantModelData.VariantId,
                    VariantName = variantModelData.VariantName,
                    MainPrice = variantModelData.VariantPriceMain,
                    SalePrice = variantModelData.VariantPriceSale,
                    Quantity = items.Find( x => x.ProductId == p.ProductId && x.VariantId == variantModelData.VariantId )!.Quantity
                } );*/
            }
        } );

        return cartDto;
    }
}