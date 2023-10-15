using System.Data;
using System.Text;
using System.Xml.Linq;
using Microsoft.Data.SqlClient;
using Dapper;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Products;

namespace BlazorElectronics.Server.Repositories.Products;

public sealed class ProductSearchRepository : DapperRepository<Product>, IProductSearchRepository
{
    const string COLUMN_ALIAS_PRODUCT_ID = "ProductIdAlias";

    const string QUERY_PARAM_CATEGORY_ID = "@categoryId";
    const string QUERY_PARAM_SEARCH_TEXT = "@searchText";
    const string QUERY_PARAM_MIN_RATING = "@minRating";
    const string QUERY_PARAM_MAX_RATING = "@maxRating";
    const string QUERY_PARAM_MIN_PRICE = "@minPrice";
    const string QUERY_PARAM_MAX_PRICE = "@maxPrice";
    const string QUERY_PARAM_SPEC_FILTER_LOOKUP = "@lookupSpecFilter";
    const string QUERY_PARAM_SPEC_FILTER_RAW = "@rawSpecFilter";
    const string QUERY_PARAM_QUERY_OFFSET = "@queryOffset";
    const string QUERY_PARAM_QUERY_ROWS = "@queryRows";

    const string XML_VARIANT_DATA_ROOT = "Variants";
    const string XML_VARIANT_DATA = "Variant";

    const string CTE_FILTERED_PRODUCTS = "FilteredProducts_CTE";
    const string CTE_PRODUCT_VARIANTS = "ProductVariants_CTE";

    const string STORED_PROCEDURE_GET_NAMES_BY_SEARCH_TEXT = "Get_ProductNamesBySearchText";
    const string STORED_PROCEDURE_GET_ALL_PRODUCTS = "Get_AllProducts";
    const string STORED_PROCEDURE_GET_PRODUCT_BY_ID = "Get_ProductById";

    public ProductSearchRepository( DapperContext dapperContext )
        : base( dapperContext ) { }

    public async Task<string?> GetProductSearchQueryString( ProductSearchRequest searchRequest )
    {
        var productQuery = new StringBuilder();
        var countQuery = new StringBuilder();

        await BuildProductSearchQuery( searchRequest, productQuery, countQuery );
        return productQuery + "-----------------------------------------" + countQuery;
    }

    public override async Task<IEnumerable<Product>?> GetAll()
    {
        await using SqlConnection connection = await _dbContext.GetOpenConnection();
        var productDictionary = new Dictionary<int, Product>();

        return await connection.QueryAsync<Product, ProductVariant, Product>
        ( STORED_PROCEDURE_GET_ALL_PRODUCTS, ( product, variant ) =>
            {
                if ( !productDictionary.TryGetValue( product.ProductId, out Product? productEntry ) )
                {
                    productEntry = product;
                    productDictionary.Add( productEntry.ProductId, productEntry );
                }
                if ( variant != null && !product.ProductVariants.Contains( variant ) )
                    product.ProductVariants.Add( variant );
                return product;
            },
            splitOn: SqlConsts.COLUMN_PRODUCT_ID,
            commandType: CommandType.StoredProcedure );
    }
    public override async Task<Product?> GetById( int id )
    {
        await using SqlConnection connection = await _dbContext.GetOpenConnection();
        var productDictionary = new Dictionary<int, Product>();

        await connection.QueryAsync<Product, ProductVariant, Product>
        ( STORED_PROCEDURE_GET_PRODUCT_BY_ID, ( product, variant ) =>
            {
                if ( !productDictionary.TryGetValue( product.ProductId, out Product? productEntry ) )
                {
                    productEntry = product;
                    productDictionary.Add( productEntry.ProductId, productEntry );
                }
                if ( variant != null && !product.ProductVariants.Contains( variant ) )
                    product.ProductVariants.Add( variant );
                return product;
            },
            splitOn: SqlConsts.COLUMN_PRODUCT_ID,
            commandType: CommandType.StoredProcedure );

        return productDictionary[ id ];
    }

    public async Task<IEnumerable<string>?> GetSearchSuggestions( string searchText )
    {
        await using SqlConnection connection = await _dbContext.GetOpenConnection();
        var searchParam = new { SearchText = searchText };
        return await connection.QueryAsync<string>(
            STORED_PROCEDURE_GET_NAMES_BY_SEARCH_TEXT, searchParam, commandType: CommandType.StoredProcedure );
    }
    public async Task<ProductSearch?> GetProductSearch( ProductSearchRequest searchRequest )
    {
        var productQuery = new StringBuilder();
        var countQuery = new StringBuilder();

        DynamicParameters dynamicParams = await BuildProductSearchQuery( searchRequest, productQuery, countQuery );

        await using SqlConnection connection = await _dbContext.GetOpenConnection();

        Task<IEnumerable<Product>?> productTask = ExecuteSearchProducts( connection, productQuery.ToString(), dynamicParams );
        //Task<int> countTask = CountProducts( connection, countQuery.ToString(), dynamicParams );

        await Task.WhenAll( productTask );

        if ( productTask.Result == null )
            return null;

        return new ProductSearch {
            Products = productTask.Result,
            //TotalSearchCount = countTask.Result
            //QueryRows = dynamicParams.Get<int>( QUERY_PARAM_QUERY_ROWS ),
            //QueryOffset = dynamicParams.Get<int>( QUERY_PARAM_QUERY_OFFSET )
        };
    }

    static async Task<DynamicParameters> BuildProductSearchQuery( ProductSearchRequest request, StringBuilder productQuery, StringBuilder countQuery )
    {
        var paramDictionary = new Dictionary<string, object>();

        await Task.Run( () =>
        {
            // FILTER PRODUCTS CTE
            productQuery.Append( $" WITH {CTE_FILTERED_PRODUCTS} AS (" );
            productQuery.Append( $" SELECT" );
            productQuery.Append( $" p.{SqlConsts.COLUMN_PRODUCT_ID} AS {COLUMN_ALIAS_PRODUCT_ID}," );
            productQuery.Append( $" p.{SqlConsts.COLUMN_PRODUCT_TITLE}," );
            productQuery.Append( $" p.{SqlConsts.COLUMN_PRODUCT_RATING}," );
            productQuery.Append( $" p.{SqlConsts.COLUMN_PRODUCT_THUMBNAIL}" );
            productQuery.Append( $" FROM {SqlConsts.TABLE_PRODUCTS} p" );

            if ( request.CategoryId != null )
            {
                productQuery.Append( $" LEFT JOIN {SqlConsts.TABLE_PRODUCT_CATEGORIES} pc" );
                productQuery.Append( $" ON p.{SqlConsts.COLUMN_PRODUCT_ID} = pc.{SqlConsts.COLUMN_PRODUCT_ID}" );
            }
            if ( !string.IsNullOrEmpty( request.SearchText ) )
            {
                productQuery.Append( $" LEFT JOIN {SqlConsts.TABLE_PRODUCT_DESCRIPTIONS} pd" );
                productQuery.Append( $" ON p.{SqlConsts.COLUMN_PRODUCT_ID} = pd.{SqlConsts.COLUMN_PRODUCT_ID}" );
            }
            if ( request.LookupSpecFilters is { Count: > 0 } )
            {
                productQuery.Append( $" LEFT JOIN {SqlConsts.TABLE_PRODUCT_SPECS_LOOKUP} psl" );
                productQuery.Append( $" ON p.{SqlConsts.COLUMN_PRODUCT_ID} = psl.{SqlConsts.COLUMN_PRODUCT_ID}" );
                productQuery.Append( $" LEFT JOIN {SqlConsts.TABLE_SPECS_LOOKUP} sl" );
                productQuery.Append( $" ON p.{SqlConsts.COLUMN_PRODUCT_ID} = sl.{SqlConsts.COLUMN_PRODUCT_ID}" );
            }
            if ( request.RawSpecFilters is { Count: > 0 } )
            {
                productQuery.Append( $" LEFT JOIN {SqlConsts.TABLE_PRODUCT_SPECS_RAW} psr" );
                productQuery.Append( $" ON p.{SqlConsts.COLUMN_PRODUCT_ID} = psr.{SqlConsts.COLUMN_PRODUCT_ID}" );
            }

            productQuery.Append( $" WHERE 1=1" );

            if ( request.CategoryId != null )
            {
                productQuery.Append( $@" AND pc.{SqlConsts.COLUMN_CATEGORY_ID} = {QUERY_PARAM_CATEGORY_ID}" );
                paramDictionary.Add( QUERY_PARAM_CATEGORY_ID, request.CategoryId.Value );
            }
            if ( !string.IsNullOrEmpty( request.SearchText ) )
            {
                productQuery.Append( $@" AND (p.{SqlConsts.COLUMN_PRODUCT_TITLE} LIKE {QUERY_PARAM_SEARCH_TEXT}" );
                productQuery.Append( $@" OR pd.{SqlConsts.COLUMN_PRODUCT_DESCR_BODY} LIKE {QUERY_PARAM_SEARCH_TEXT})" );
                paramDictionary.Add( QUERY_PARAM_SEARCH_TEXT, $"%{request.SearchText}%" );
            }
            if ( request.MinRating != null )
            {
                productQuery.Append( $@" AND p.{SqlConsts.COLUMN_PRODUCT_RATING} >= {QUERY_PARAM_MIN_RATING}" );
                paramDictionary.Add( QUERY_PARAM_MIN_RATING, request.MinRating );
            }
            if ( request.MaxRating != null )
            {
                productQuery.Append( $@" AND p.{SqlConsts.COLUMN_PRODUCT_RATING} <= {QUERY_PARAM_MAX_RATING}" );
                paramDictionary.Add( QUERY_PARAM_MAX_RATING, request.MaxRating );
            }
            if ( request.LookupSpecFilters is { Count: > 0 } ) { }
            if ( request.RawSpecFilters is { Count: > 0 } )
            {
                //productQuery.Append( $" AND p.{COLUMN_PRODUCT_ID} = psr.{COLUMN_PRODUCT_ID}" );
                for ( int i = 0; i < request.RawSpecFilters.Count; i++ )
                {
                    string rawParamName = QUERY_PARAM_SPEC_FILTER_RAW + i;
                    productQuery.Append( $@" AND psr.{request.RawSpecFilters[ i ].SpecName} = {rawParamName}" );
                    paramDictionary.Add( rawParamName, request.RawSpecFilters[ i ].SpecValue );
                }
            }

            productQuery.Append( ")," );

            // PRODUCT VARIANTS CTE
            productQuery.Append( $" {CTE_PRODUCT_VARIANTS} AS (" );
            productQuery.Append( $" SELECT" );
            productQuery.Append( $" pv.{SqlConsts.COLUMN_PRODUCT_ID} AS {COLUMN_ALIAS_PRODUCT_ID}," );
            productQuery.Append( $" pv.{SqlConsts.COLUMN_VARIANT_ID_PRIMARY}," );
            productQuery.Append( $" pv.{SqlConsts.COLUMN_VARIANT_ID}," );
            productQuery.Append( $" pv.{SqlConsts.COLUMN_VARIANT_NAME}," );
            productQuery.Append( $" pv.{SqlConsts.COLUMN_VARIANT_PRICE_MAIN}," );
            productQuery.Append( $" pv.{SqlConsts.COLUMN_VARIANT_PRICE_SALE}" );
            productQuery.Append( $" FROM {SqlConsts.TABLE_PRODUCT_VARIANTS} pv" );
            productQuery.Append( $" )" );

            // PRODUCT VARIANTS QUERY SELECTION
            productQuery.Append( $" SELECT" );
            productQuery.Append( $" f.{COLUMN_ALIAS_PRODUCT_ID} AS {SqlConsts.COLUMN_PRODUCT_ID}," );
            productQuery.Append( $" f.{SqlConsts.COLUMN_PRODUCT_TITLE}," );
            productQuery.Append( $" f.{SqlConsts.COLUMN_PRODUCT_RATING}," );
            productQuery.Append( $" f.{SqlConsts.COLUMN_PRODUCT_THUMBNAIL}," );
            productQuery.Append( $" (" );
            productQuery.Append( $" SELECT" );
            productQuery.Append( $" pv.{SqlConsts.COLUMN_VARIANT_ID_PRIMARY}," );
            productQuery.Append( $" pv.{SqlConsts.COLUMN_VARIANT_ID}," );
            productQuery.Append( $" pv.{SqlConsts.COLUMN_VARIANT_NAME}," );
            productQuery.Append( $" pv.{SqlConsts.COLUMN_VARIANT_PRICE_MAIN}," );
            productQuery.Append( $" pv.{SqlConsts.COLUMN_VARIANT_PRICE_SALE}" );
            productQuery.Append( $" FROM {CTE_PRODUCT_VARIANTS} pv" );
            productQuery.Append( $" WHERE f.{COLUMN_ALIAS_PRODUCT_ID} = pv.{COLUMN_ALIAS_PRODUCT_ID}" );
            productQuery.Append( $" FOR XML PATH('{XML_VARIANT_DATA}'), ROOT('{XML_VARIANT_DATA_ROOT}'), TYPE" );
            productQuery.Append( $" ) AS Variant" );
            productQuery.Append( $" FROM {CTE_FILTERED_PRODUCTS} f" );

            // PRODUCT VARIANTS QUERY CONDITIONS
            productQuery.Append( $" WHERE EXISTS (" );
            productQuery.Append( $" SELECT *" );
            productQuery.Append( $" FROM {CTE_PRODUCT_VARIANTS} pv" );
            productQuery.Append( $" WHERE f.{COLUMN_ALIAS_PRODUCT_ID} = pv.{COLUMN_ALIAS_PRODUCT_ID}" );

            if ( request.MinPrice != null || request.MaxPrice != null )
            {
                if ( request.MinPrice != null )
                {
                    productQuery.Append( $@" AND (pv.{SqlConsts.COLUMN_VARIANT_PRICE_MAIN} >= {QUERY_PARAM_MIN_PRICE}" );
                    productQuery.Append( $@" OR pv.{SqlConsts.COLUMN_VARIANT_PRICE_SALE} >= {QUERY_PARAM_MIN_PRICE})" );
                    paramDictionary.Add( QUERY_PARAM_MIN_PRICE, request.MinPrice.Value );
                }
                if ( request.MaxPrice != null )
                {
                    productQuery.Append( $@" AND (pv.{SqlConsts.COLUMN_VARIANT_PRICE_MAIN} <= {QUERY_PARAM_MAX_PRICE}" );
                    productQuery.Append( $@" OR pv.{SqlConsts.COLUMN_VARIANT_PRICE_SALE} <= {QUERY_PARAM_MAX_PRICE})" );
                    paramDictionary.Add( QUERY_PARAM_MAX_PRICE, request.MaxPrice.Value );
                }
            }

            productQuery.Append( $" )" );

            // FINAL PAGINATION
            productQuery.Append( $" ORDER BY f.{COLUMN_ALIAS_PRODUCT_ID}" );
            productQuery.Append( $@" OFFSET {QUERY_PARAM_QUERY_OFFSET} ROWS" );
            productQuery.Append( $@" FETCH NEXT {QUERY_PARAM_QUERY_ROWS} ROWS ONLY;" );
            paramDictionary.Add( QUERY_PARAM_QUERY_OFFSET, Math.Max( request.Page - 1, 0 ) * request.Rows );
            paramDictionary.Add( QUERY_PARAM_QUERY_ROWS, request.Rows );
        } );

        return new DynamicParameters( paramDictionary );
    }
    static async Task<int> CountProducts( SqlConnection connection, string dynamicQuery, DynamicParameters dynamicParams )
    {
        return await connection.QuerySingleAsync<int>( dynamicQuery, dynamicParams, commandType: CommandType.Text );
    }
    static async Task<IEnumerable<Product>?> ExecuteSearchProducts( SqlConnection connection, string dynamicQuery, DynamicParameters dynamicParams )
    {
        var productDictionary = new Dictionary<int, Product>();

        await connection.QueryAsync<Product, string, Product>
        ( dynamicQuery, ( product, variantXml ) =>
            {
                if ( !productDictionary.TryGetValue( product.ProductId, out Product? productEntry ) )
                {
                    productEntry = product;
                    productDictionary.Add( productEntry.ProductId, productEntry );
                }
                if ( variantXml != null )
                    productEntry.ProductVariants = ParseVariantData( variantXml );
                return productEntry;
            },
            dynamicParams,
            splitOn: XML_VARIANT_DATA,
            commandType: CommandType.Text );

        return productDictionary.Values;
    }
    static List<ProductVariant> ParseVariantData( string variantXml )
    {
        if ( string.IsNullOrEmpty( variantXml ) )
            return new List<ProductVariant>();

        XDocument doc = XDocument.Parse( variantXml );

        if ( doc.Root == null )
            return new List<ProductVariant>();

        var variantsList = new List<ProductVariant>();

        foreach ( XElement e in doc.Root.Elements( XML_VARIANT_DATA ) )
        {
            var variant = new ProductVariant();

            if ( e.Element( SqlConsts.COLUMN_VARIANT_ID ) == null ||
                 e.Element( SqlConsts.COLUMN_VARIANT_NAME ) == null ||
                 e.Element( SqlConsts.COLUMN_VARIANT_PRICE_MAIN ) == null ||
                 e.Element( SqlConsts.COLUMN_VARIANT_PRICE_SALE ) == null )
            {
                variant.VariantName = "NULL VARIANT DATA!";
                variantsList.Add( variant );
                continue;
            }

            variant.VariantId = ( int ) e.Element( SqlConsts.COLUMN_VARIANT_ID )!;
            variant.VariantName = ( string ) e.Element( SqlConsts.COLUMN_VARIANT_NAME )!;
            variant.VariantPriceMain = ( decimal ) e.Element( SqlConsts.COLUMN_VARIANT_PRICE_MAIN )!;
            variant.VariantPriceSale = ( decimal ) e.Element( SqlConsts.COLUMN_VARIANT_PRICE_SALE )!;

            variantsList.Add( variant );
        }

        return variantsList;
    }
}