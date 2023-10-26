using BlazorElectronics.Server.Models.Cart;
using BlazorElectronics.Server.Models.Products;
using BlazorElectronics.Server.Repositories.Cart;
using BlazorElectronics.Server.Services.Users;
using BlazorElectronics.Shared.Inbound.Cart;
using BlazorElectronics.Shared.Mutual;

namespace BlazorElectronics.Server.Services.Cart;

public class CartService : ICartService
{
    readonly ICartRepository _cartRepository;
    readonly IUserAccountService _userAccountService;

    public CartService( ICartRepository cartRepository, IUserAccountService userAccountService )
    {
        _cartRepository = cartRepository;
        _userAccountService = userAccountService;
    }
    
    public async Task<ServiceResponse<Cart_DTO?>> PostCartItems( int userId, List<CartItemId_DTO> cartItemsDtos )
    {
        // Insert New Items
        List<CartItem> models = MapItemIdsToModels( cartItemsDtos, userId );
        bool inserted = await _cartRepository.InsertItems( models );

        if ( !inserted )
            return new ServiceResponse<Cart_DTO?>( null, false, "Failed to insert Cart Items into database!" );
        
        // Get All Items
        IEnumerable<CartItem>? allItems = await _cartRepository.GetCartItems( userId );

        if ( allItems == null )
            return new ServiceResponse<Cart_DTO?>( null, false, $"Failed to retrieve Cart Items for user {userId}!" );
        
        // Get Products
        GetProductIdsFromModels( allItems, out List<int> productIds, out List<int> variantIds );
        IEnumerable<Product>? cartProducts = await _cartRepository.GetCartProducts( productIds, variantIds );

        if ( cartProducts == null )
            return new ServiceResponse<Cart_DTO?>( null, false, $"Failed to retrieve Cart Products for user {userId}" );
        
        // Return Dto
        Cart_DTO cartDto = await MapProductsToCart( cartProducts, cartItemsDtos );
        return new ServiceResponse<Cart_DTO?>( cartDto, true, "Successfully inserted Cart Items to database, and retrieved Cart Products from database." );
    }
    public async Task<ServiceResponse<bool>> AddToCart( int userId, CartItemId_DTO cartItem )
    {
        CartItem modelItem = MapItemIdsToModel( cartItem, userId );

        bool response = await _cartRepository.InsertItem( modelItem );

        return !response 
            ? new ServiceResponse<bool>( false, false, "Failed to insert Cart Item into database!" ) 
            : new ServiceResponse<bool>( true, true, "Successfully inserted Cart Item into database." );
    }
    public async Task<ServiceResponse<bool>> UpdateQuantity( int userId, CartItemId_DTO cartItem )
    {
        CartItem modelItem = MapItemIdsToModel( cartItem, userId );

        bool response = await _cartRepository.UpdateItemQuantity( modelItem );

        return !response
            ? new ServiceResponse<bool>( false, false, "Failed to update Cart Item Quantity in database!" )
            : new ServiceResponse<bool>( true, true, "Successfully updated Cart Item Quantity in database." );
    }
    public async Task<ServiceResponse<bool>> RemoveFromCart( int userId, CartItemId_DTO cartItem )
    {
        CartItem modelItem = MapItemIdsToModel( cartItem, userId );

        bool response = await _cartRepository.RemoveItem( modelItem );

        return !response
            ? new ServiceResponse<bool>( false, false, "Failed to remove Cart Item from database!" )
            : new ServiceResponse<bool>( true, true, "Successfully removed Cart Item from database." );
    }
    public async Task<ServiceResponse<int>> CountCartItems( int userId )
    {
        int count = await _cartRepository.CountCartItems( userId );

        return count < 0
            ? new ServiceResponse<int>( -1, false, $"Failed to find items for user {userId}" )
            : new ServiceResponse<int>( count, true, $"Found cart items for user {userId}" );
    }
    public async Task<ServiceResponse<Cart_DTO?>> GetCartProducts( int userId )
    {
        IEnumerable<CartItem>? cartItems = await _cartRepository.GetCartItems( userId );

        if ( cartItems == null )
            return new ServiceResponse<Cart_DTO?>( null, false, $"Failed to retrieve Cart Items for user {userId}!" );

        List<CartItem> itemsList = cartItems.ToList();

        GetProductIdsFromModels( itemsList, out List<int> productIds, out List<int> variantIds );
        IEnumerable<Product>? cartProducts = await _cartRepository.GetCartProducts( productIds, variantIds );

        if ( cartProducts == null )
            return new ServiceResponse<Cart_DTO?>( null, false, "Failed to retrieve Cart Products from database!" );

        Cart_DTO cartDto = await MapProductsToCart( cartProducts, itemsList.ToList() );

        return new ServiceResponse<Cart_DTO?>( cartDto, true, "Successfully retrieved Cart Products from database." );
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