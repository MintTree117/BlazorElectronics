namespace BlazorElectronics.Client;

public static class Routes
{
    public const string HOME = "/";
    public const string PRODUCT = "/product";
    public const string PRODUCT_EDIT = "/admin/product-edit";
    public const string SEARCH = "/search";
    public const string FEATURES = $"{SEARCH}?{PARAM_SEARCH_FEATURED}";
    public const string SALES = $"{SEARCH}?{PARAM_SEARCH_SALES}";
    public const string FEATURED_DEALS = $"{SEARCH}?{PARAM_SEARCH_FEATURED_DEALS}";
    public const string SEARCH_VENDOR = $"{SEARCH}?{PARAM_SEARCH_VENDOR}";
    public const string REVIEW = "/review-product";
    public const string LOGIN = "/login";
    public const string LOGOUT = "/logout";
    public const string REGISTERED = "/registered";
    public const string ACCOUNT_DETAILS = "/account/details";
    public const string ACCOUNT_SESSIONS = "/account/sessions";
    public const string ACCOUNT_ORDERS = "/account/orders";
    public const string ACCOUNT_ORDER_DETAILS = "/account/order-details";
    public const string CART = "/cart";
    public const string CHECKOUT = "/checkout";
    
    public const string BULK_CATEGORY = "";
    public const string BULK_KEYS = "";
    public const string CRUD_CATEGORY = "";
    public const string CRUD_FEATURES = "";
    public const string CRUD_SPECS = "";
    public const string CRUD_VENDORS = "";
    public const string SEED_DB = "";

    public const string REGISTERED_EMAIL = "registered-email";
    public const string VERIFY_TOKEN = "token";
    public const string PARAM_SEARCH_TEXT = "search-text";
    public const string PARAM_PRODUCT_EDIT_ID = "productId";
    public const string PARAM_SEARCH_FEATURED_DEALS = "featured-deals";
    public const string PARAM_SEARCH_VENDOR = "vendorId";
    public const string PARAM_SEARCH_SALES = "on-sale";
    public const string PARAM_SEARCH_FEATURED = "featured";
}