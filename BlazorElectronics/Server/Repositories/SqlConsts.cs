namespace BlazorElectronics.Server.Repositories;

public static class SqlConsts
{
    public const string TABLE_PRODUCTS = "Products";
    public const string TABLE_PRODUCT_CATEGORIES = "ProductCategories";
    public const string TABLE_PRODUCT_DESCRIPTIONS = "ProductDescriptions";
    public const string TABLE_PRODUCT_SPECS_LOOKUP = "ProductSpecsLookup";
    public const string TABLE_PRODUCT_SPECS_RAW = "ProductSpecsRaw";
    public const string TABLE_PRODUCT_VARIANTS = "ProductVariants";
    public const string TABLE_SPECS_LOOKUP = "SpecsLookup";

    public const string COLUMN_PRODUCT_ID = "ProductId";
    public const string COLUMN_PRODUCT_TITLE = "ProductTitle";
    public const string COLUMN_PRODUCT_RATING = "ProductRating";
    public const string COLUMN_PRODUCT_IMAGE_ID = "ImageId";
    public const string COLUMN_PRODUCT_REVIEW_ID = "ReviewId";
    public const string COLUMN_PRODUCT_THUMBNAIL = "ProductThumbnail";
    public const string COLUMN_PRODUCT_DESCRIPTION_ID_COLUMN = "DescriptionId";
    public const string COLUMN_PRODUCT_DESCR_BODY = "DescriptionBody";
    
    public const string COLUMN_VARIANT_ID_PRIMARY = "ProductVariantId";
    public const string COLUMN_VARIANT_ID = "VariantId";
    public const string COLUMN_VARIANT_NAME = "VariantName";
    public const string COLUMN_VARIANT_PRICE_MAIN = "VariantPriceMain";
    public const string COLUMN_VARIANT_PRICE_SALE = "VariantPriceSale";
    
    public const string COLUMN_CATEGORY_ID = "CategoryId";
    public const string COLUMN_CATEGORY_SUB_ID = "CategoryId";
}