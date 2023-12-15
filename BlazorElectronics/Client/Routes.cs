namespace BlazorElectronics.Client;

public static class Routes
{
    public const string HOME = "/";
    public const string PRODUCT = "/product";
    public const string SEARCH = "/search";
    public const string FEATURES = $"{SEARCH}?featured=true";
    public const string SALES = $"{SEARCH}?sales=true";
    public const string LOGIN = "/login";
    public const string LOGOUT = "/logout";
    public const string ACCOUNT = "/account";
    public const string ORDERS = "/account/orders";
    public const string CART = "/cart";
    
    public const string BULK_CATEGORY = "";
    public const string BULK_KEYS = "";
    public const string CRUD_CATEGORY = "";
    public const string CRUD_FEATURES = "";
    public const string CRUD_SPECS = "";
    public const string CRUD_VENDORS = "";
    public const string SEED_DB = "";

    public const string SEARCH_TEXT_PARAM = "searchText";
}