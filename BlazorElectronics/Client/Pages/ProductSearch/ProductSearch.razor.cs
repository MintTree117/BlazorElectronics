using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using BlazorElectronics.Shared.Inbound.Products;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.ProductSearch;

public partial class ProductSearch : PageView
{
    [Parameter]
    public string? Primary { get; set; }
    [Parameter]
    public string? Secondary { get; set; }
    [Parameter]
    public string? Tertiary { get; set; }

    string _pageTitle = "All Products";
    
    protected override void OnInitialized()
    {
    }
    public void Dispose()
    {
    }
    
    
    protected override async Task OnParametersSetAsync()
    {
        /*Uri uri = NavigationManager.ToAbsoluteUri( NavigationManager.Uri );
        NameValueCollection queryParameters = HttpUtility.ParseQueryString( uri.Query );

        ProductSearchRequest filters = MapSearchQueryParameters( queryParameters );
        
        await ProductService.SearchProductsByCategory( filters, Primary, Secondary, Tertiary );
        
        StateHasChanged();*/
    }

    ProductSearchRequest MapSearchQueryParameters( NameValueCollection queryParameters )
    {
        var filtersDTO = new ProductSearchRequest();
        Type type = typeof( ProductSearchRequest );

        foreach ( string? key in queryParameters.AllKeys )
        {
            if ( string.IsNullOrEmpty( key ) )
                continue;

            var parsedKeyBuilder = new StringBuilder();
            parsedKeyBuilder.Append( char.ToUpper( key[ 0 ] ) );

            for ( int i = 1; i < key.Length; i++ )
            {
                if ( key[ i ] == '-' )
                    continue;
                if ( key[ i - 1 ] == '-' )
                {
                    parsedKeyBuilder.Append( char.ToUpper( key[ i ] ) );
                    continue;
                }
                parsedKeyBuilder.Append( char.ToLower( key[ i ] ) );
            }

            string filterName = parsedKeyBuilder.ToString();
            var stringValue = queryParameters[ key ];
            PropertyInfo? propertyInfo = type.GetProperty( filterName );

            if ( propertyInfo != null )
            {
                var convertedValue = Convert.ChangeType( stringValue, propertyInfo.PropertyType );
                propertyInfo.SetValue( filtersDTO, convertedValue );
                continue;
            }

            /*filtersDTO.SpecFilters.Add( new ProductSpecFilter_DTO {
                SpecName = key,
                SpecValue = Convert.ChangeType( stringValue, typeof( object ) )
            } );*/
        }

        return filtersDTO;
    }
}