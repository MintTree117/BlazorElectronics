namespace BlazorElectronics.Server.Repositories.Products;

public class TEMP
{
    /*static async Task<DynamicParameters> BuildProductSearchQuery( CategoryIdMap categoryIdMap, ProductSearchRequest request, StringBuilder productQuery, StringBuilder countQuery )
    {
        var paramDictionary = new Dictionary<string, object>();

        await Task.Run( () =>
        {
            var firstHalf = new StringBuilder();
            var secondHalf = new StringBuilder();

            // FILTER PRODUCTS CTE
            firstHalf.Append( $" WITH {CTE_FILTERED_PRODUCTS} AS (" );
            firstHalf.Append( $" SELECT" );
            firstHalf.Append( $" p.{DBConsts.COL_PRODUCT_ID} AS {COLUMN_ALIAS_PRODUCT_ID}," );
            firstHalf.Append( $" p.{DBConsts.COL_PRODUCT_TITLE}," );
            firstHalf.Append( $" p.{DBConsts.COL_PRODUCT_RATING}," );
            firstHalf.Append( $" p.{DBConsts.COL_PRODUCT_THUMBNAIL}" );
            firstHalf.Append( $" FROM {DBConsts.TABLE_PRODUCTS} p" );

            if ( request.CategoryId != null )
            {
                firstHalf.Append( $" LEFT JOIN {DapperConsts.TABLE_PRODUCT_CATEGORIES} pc" );
                firstHalf.Append( $" ON p.{DapperConsts.COLUMN_PRODUCT_ID} = pc.{DapperConsts.COLUMN_PRODUCT_ID}" );
            }
            if ( !string.IsNullOrEmpty( request.SearchText ) )
            {
                firstHalf.Append( $" LEFT JOIN {DBConsts.TABLE_PRODUCT_DESCRIPTIONS} pd" );
                firstHalf.Append( $" ON p.{DBConsts.COL_PRODUCT_ID} = pd.{DBConsts.COL_PRODUCT_ID}" );
            }
            if ( request.LookupSpecFilters is { Count: > 0 } )
            {
                firstHalf.Append( $" LEFT JOIN {DBConsts.TABLE_PRODUCT_SPECS_LOOKUP} psl" );
                firstHalf.Append( $" ON p.{DBConsts.COL_PRODUCT_ID} = psl.{DBConsts.COL_PRODUCT_ID}" );
                firstHalf.Append( $" LEFT JOIN {DBConsts.TABLE_SPECS_LOOKUP} sl" );
                firstHalf.Append( $" ON p.{DBConsts.COL_PRODUCT_ID} = sl.{DBConsts.COL_PRODUCT_ID}" );
            }
            if ( request.RawSpecFilters is { Count: > 0 } )
            {
                firstHalf.Append( $" LEFT JOIN {DBConsts.TABLE_PRODUCT_SPECS_RAW} psr" );
                firstHalf.Append( $" ON p.{DBConsts.COL_PRODUCT_ID} = psr.{DBConsts.COL_PRODUCT_ID}" );
            }

            firstHalf.Append( $" WHERE 1=1" );

            if ( request.CategoryId != null )
            {
                firstHalf.Append( $@" AND pc.{DapperConsts.COLUMN_CATEGORY_PRIMARY_ID} = {QUERY_PARAM_CATEGORY_ID}" );
                paramDictionary.Add( QUERY_PARAM_CATEGORY_ID, request.CategoryId.Value );
            }
            if ( !string.IsNullOrEmpty( request.SearchText ) )
            {
                firstHalf.Append( $@" AND (p.{DBConsts.COL_PRODUCT_TITLE} LIKE {QUERY_PARAM_SEARCH_TEXT}" );
                firstHalf.Append( $@" OR pd.{DBConsts.COL_PRODUCT_DESCR_BODY} LIKE {QUERY_PARAM_SEARCH_TEXT})" );
                paramDictionary.Add( QUERY_PARAM_SEARCH_TEXT, $"%{request.SearchText}%" );
            }
            if ( request.MinRating != null )
            {
                firstHalf.Append( $@" AND p.{DBConsts.COL_PRODUCT_RATING} >= {QUERY_PARAM_MIN_RATING}" );
                paramDictionary.Add( QUERY_PARAM_MIN_RATING, request.MinRating );
            }
            if ( request.MaxRating != null )
            {
                firstHalf.Append( $@" AND p.{DBConsts.COL_PRODUCT_RATING} <= {QUERY_PARAM_MAX_RATING}" );
                paramDictionary.Add( QUERY_PARAM_MAX_RATING, request.MaxRating );
            }
            if ( request.LookupSpecFilters is { Count: > 0 } ) { }
            if ( request.RawSpecFilters is { Count: > 0 } )
            {
                //productQuery.Append( $" AND p.{COLUMN_PRODUCT_ID} = psr.{COLUMN_PRODUCT_ID}" );
                for ( int i = 0; i < request.RawSpecFilters.Count; i++ )
                {
                    string rawParamName = QUERY_PARAM_SPEC_FILTER_RAW + i;
                    firstHalf.Append( $@" AND psr.{request.RawSpecFilters[ i ].SpecName} = {rawParamName}" );
                    paramDictionary.Add( rawParamName, request.RawSpecFilters[ i ].SpecValue );
                }
            }

            firstHalf.Append( ")," );

            // PRODUCT VARIANTS CTE
            firstHalf.Append( $" {CTE_PRODUCT_VARIANTS} AS (" );
            firstHalf.Append( $" SELECT" );
            firstHalf.Append( $" pv.{DBConsts.COL_PRODUCT_ID} AS {COLUMN_ALIAS_PRODUCT_ID}," );
            firstHalf.Append( $" pv.{DBConsts.COL_VARIANT_ID}," );
            firstHalf.Append( $" pv.{DBConsts.COL_VARIANT_SUB_ID}," );
            firstHalf.Append( $" pv.{DBConsts.COL_VARIANT_NAME}," );
            firstHalf.Append( $" pv.{DBConsts.COL_VARIANT_PRICE_ORIGINAL}," );
            firstHalf.Append( $" pv.{DBConsts.COL_VARIANT_PRICE_SALE}" );
            firstHalf.Append( $" FROM {DBConsts.TABLE_PRODUCT_VARIANTS} pv" );
            firstHalf.Append( $" )" );

            // PRODUCT VARIANTS QUERY SELECTION
            firstHalf.Append( $" SELECT" );

            countQuery.Append( firstHalf );
            countQuery.Append( $" COUNT(*) OVER () AS TotalCount," );

            productQuery.Append( firstHalf );
            productQuery.Append( $" f.{COLUMN_ALIAS_PRODUCT_ID} AS {DBConsts.COL_PRODUCT_ID}," );
            productQuery.Append( $" f.{DBConsts.COL_PRODUCT_TITLE}," );
            productQuery.Append( $" f.{DBConsts.COL_PRODUCT_RATING}," );
            productQuery.Append( $" f.{DBConsts.COL_PRODUCT_THUMBNAIL}," );

            secondHalf.Append( $" (" );
            secondHalf.Append( $" SELECT" );
            secondHalf.Append( $" pv.{DBConsts.COL_VARIANT_ID}," );
            secondHalf.Append( $" pv.{DBConsts.COL_VARIANT_SUB_ID}," );
            secondHalf.Append( $" pv.{DBConsts.COL_VARIANT_NAME}," );
            secondHalf.Append( $" pv.{DBConsts.COL_VARIANT_PRICE_ORIGINAL}," );
            secondHalf.Append( $" pv.{DBConsts.COL_VARIANT_PRICE_SALE}" );
            secondHalf.Append( $" FROM {CTE_PRODUCT_VARIANTS} pv" );
            secondHalf.Append( $" WHERE f.{COLUMN_ALIAS_PRODUCT_ID} = pv.{COLUMN_ALIAS_PRODUCT_ID}" );
            secondHalf.Append( $" FOR XML PATH('{XML_VARIANT_DATA}'), ROOT('{XML_VARIANT_DATA_ROOT}'), TYPE" );
            secondHalf.Append( $" ) AS Variant" );
            secondHalf.Append( $" FROM {CTE_FILTERED_PRODUCTS} f" );

            // PRODUCT VARIANTS QUERY CONDITIONS
            secondHalf.Append( $" WHERE EXISTS (" );
            secondHalf.Append( $" SELECT *" );
            secondHalf.Append( $" FROM {CTE_PRODUCT_VARIANTS} pv" );
            secondHalf.Append( $" WHERE f.{COLUMN_ALIAS_PRODUCT_ID} = pv.{COLUMN_ALIAS_PRODUCT_ID}" );

            if ( request.MinPrice != null || request.MaxPrice != null )
            {
                if ( request.MinPrice != null )
                {
                    secondHalf.Append( $@" AND (pv.{DBConsts.COL_VARIANT_PRICE_ORIGINAL} >= {QUERY_PARAM_MIN_PRICE}" );
                    secondHalf.Append( $@" OR pv.{DBConsts.COL_VARIANT_PRICE_SALE} >= {QUERY_PARAM_MIN_PRICE})" );
                    paramDictionary.Add( QUERY_PARAM_MIN_PRICE, request.MinPrice.Value );
                }
                if ( request.MaxPrice != null )
                {
                    secondHalf.Append( $@" AND (pv.{DBConsts.COL_VARIANT_PRICE_ORIGINAL} <= {QUERY_PARAM_MAX_PRICE}" );
                    secondHalf.Append( $@" OR pv.{DBConsts.COL_VARIANT_PRICE_SALE} <= {QUERY_PARAM_MAX_PRICE})" );
                    paramDictionary.Add( QUERY_PARAM_MAX_PRICE, request.MaxPrice.Value );
                }
            }

            secondHalf.Append( $" )" );

            countQuery.Append( secondHalf );
            productQuery.Append( secondHalf );

            // FINAL PAGINATION
            productQuery.Append( $" ORDER BY f.{COLUMN_ALIAS_PRODUCT_ID}" );
            productQuery.Append( $@" OFFSET {QUERY_PARAM_QUERY_OFFSET} ROWS" );
            productQuery.Append( $@" FETCH NEXT {QUERY_PARAM_QUERY_ROWS} ROWS ONLY;" );
            paramDictionary.Add( QUERY_PARAM_QUERY_OFFSET, Math.Max( request.Page - 1, 0 ) * request.Rows );
            paramDictionary.Add( QUERY_PARAM_QUERY_ROWS, request.Rows );
        } );

        return new DynamicParameters( paramDictionary );
    }*/
}