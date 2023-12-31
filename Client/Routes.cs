namespace BlazorElectronics.Client;

public static class Routes
{
    public const string HOME = "/";
    public const string PRODUCT = "/product";
    public const string SEARCH = "/search";
    public const string FEATURES = $"{SEARCH}?featured=true";
    public const string REVIEW = "/review-product";
    public const string SALES = $"{SEARCH}?sales=true";
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

    public const string REGISTERED_EMAIL = "registeredEmail";
    public const string VERIFY_TOKEN = "token";
    public const string SEARCH_TEXT_PARAM = "searchText";
    public const string PRODUCT_EDIT_NEW_PARAM = "newProduct";
}