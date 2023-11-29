using System.Net.Http.Json;
using System.Text;
using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DtosOutbound.Products;
using BlazorElectronics.Shared.Outbound.Products;
using BlazorElectronics.Shared.Products;
using BlazorElectronics.Shared.Products.Search;

namespace BlazorElectronics.Client.Services.Products;

public class ProductServiceClient : ClientService, IProductServiceClient
{
    public event Action<string>? ExceptionEvent;
    public event Action<ApiReply<ProductSearchResponse?>?>? ProductSearchChanged;
    public event Action<string>? ProductSearchNullabillityTest;

    string? CategoryUrl;
    string? SearchText;
    public ProductSearchRequest? SearchRequest { get; set; }

    const string SERVER_RESPONSE_MESSAGE_NULL = "Server returned null!";
    const string SERVER_RESPONSE_MESSAGE_FAILURE_NO_MESSAGE = "Server returned null data without a return message!";
    const string CONTROLLER_URL = "api/Product/";
    const string PRODUCT_SEARCH_SUGGESTIONS_URL = "api/Product/search-suggestions/";
    const string PRODUCT_SEARCH_CATEGORY_TEXT_URL = "api/Product/search-category-text/";
    const string PRODUCT_SEARCH_CATEGORY_URL = "api/Product/search-category";
    const string PRODUCT_SEARCH_TEXT_URL = "api/Product/search-text";
    const string PRODUCT_DETAILS_URL = "api/Product/details";

    public ProductServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }

    public void ClearSearchRequest()
    {
        CategoryUrl = null;
        SearchText = null;
        SearchRequest = null;
    }
    public void UpdateSearchCategory( string url )
    {
        SearchRequest ??= new ProductSearchRequest();
        CategoryUrl = url;

        if ( SearchRequest == null )
            ProductSearchNullabillityTest?.Invoke( "On Category" );
    }
    public void UpdateSearchText( string text )
    {
        SearchRequest ??= new ProductSearchRequest();
        SearchText = text;

        if ( SearchRequest == null )
            ProductSearchNullabillityTest?.Invoke( "On Text" );
    }
    public void UpdateSearchPage( int page )
    {
        SearchRequest ??= new ProductSearchRequest();
        SearchRequest.Page = page;

        if ( SearchRequest == null )
            ProductSearchNullabillityTest?.Invoke( "On Page" );
    }
    public void UpdateSearchResultsCount( int count )
    {
        SearchRequest ??= new ProductSearchRequest();
        SearchRequest.Rows = count;

        if ( SearchRequest == null )
            ProductSearchNullabillityTest?.Invoke( "On Count" );
    }
    public string? GetProductSearchUrl()
    {
        if ( SearchRequest == null )
            ProductSearchNullabillityTest?.Invoke( "On Search" );
        
        SearchRequest ??= new ProductSearchRequest();
        
        var urlBuilder = new StringBuilder();

        if ( string.IsNullOrEmpty( CategoryUrl ) && string.IsNullOrEmpty( SearchText ) )
            return null;

        if ( !string.IsNullOrEmpty( CategoryUrl ) )
        {
            urlBuilder.Append( $"/{CategoryUrl}/" );

            if ( !string.IsNullOrEmpty( SearchText ) )
                urlBuilder.Append( $"{SearchText}/" );
        }
        else if ( !string.IsNullOrEmpty( SearchText ) )
        {
            urlBuilder.Append( $"/{SearchText}/" );
        }
        
        urlBuilder.Append( $"?page={SearchRequest.Page}" );
        urlBuilder.Append( $"&rows={SearchRequest.Rows}" );

        if ( SearchRequest.MinPrice != null )
            urlBuilder.Append( $"&minPrice={SearchRequest.MinPrice.Value}" );
        if ( SearchRequest.MaxPrice != null )
            urlBuilder.Append( $"&maxPrice={SearchRequest.MaxPrice.Value}" );
        if ( SearchRequest.MinRating != null )
            urlBuilder.Append( $"&minRating={SearchRequest.MinRating.Value}" );
        if ( SearchRequest.MaxRating != null )
            urlBuilder.Append( $"&maxRating={SearchRequest.MaxRating.Value}" );

        /*foreach ( ProductSpecFilter_DTO specFilter in SearchRequest.SpecFilters )
        {
            if ( string.IsNullOrEmpty( specFilter.SpecName ) || specFilter.SpecValue == null )
                continue;
            urlBuilder.Append( $"&{specFilter.SpecName}={specFilter.SpecValue}" );
        }*/
        
        return urlBuilder.ToString();
    }
    
    public async Task<ApiReply<ProductSuggestionsResponse?>> GetProductSearchSuggestions( string searchText )
    {
        string url = $"{PRODUCT_SEARCH_SUGGESTIONS_URL}{searchText}";

        try
        {
            var response = await Http.GetFromJsonAsync<ApiReply<ProductSuggestionsResponse?>>( url );

            if ( response == null )
                return new ApiReply<ProductSuggestionsResponse?>( null, false, "Service response is null!" );

            return !response.Success
                ? new ApiReply<ProductSuggestionsResponse?>( null, false, response.Message )
                : response;
        }
        catch ( Exception e )
        {
            return new ApiReply<ProductSuggestionsResponse?>( null, false, e.Message );
        }
    }
    public async Task SearchProductsByCategory( ProductSearchRequest filters, string primary, string? secondary = null, string? tertiary = null )
    {
        try
        {
            var urlBuilder = new StringBuilder( CONTROLLER_URL );
            urlBuilder.Append( primary );

            if ( !string.IsNullOrEmpty( secondary ) )
                urlBuilder.Append( secondary );

            if ( !string.IsNullOrEmpty( tertiary ) )
                urlBuilder.Append( tertiary );

            urlBuilder.Append( "?filters=" );
            urlBuilder.Append( Uri.EscapeDataString( Newtonsoft.Json.JsonConvert.SerializeObject( filters ) ) );

            await SearchProducts( urlBuilder.ToString() );
        }
        catch ( Exception e )
        {
            ExceptionEvent?.Invoke( e.Message );
        }
    }
    public async Task<ApiReply<ProductDetailsResponse?>> GetProductDetails( int productId )
    {
        try
        {
            string url = $"{PRODUCT_DETAILS_URL}/{productId}";
            var response = await Http.GetFromJsonAsync<ApiReply<ProductDetailsResponse?>>( url );

            if ( response == null )
                return new ApiReply<ProductDetailsResponse?>( null, false, "Service response is null!" );

            return !response.Success
                ? new ApiReply<ProductDetailsResponse?>( null, false, response.Message )
                : response;
        }
        catch ( Exception e )
        {
            return new ApiReply<ProductDetailsResponse?>( null, false, e.Message );
        }
    }
    
    async Task SearchProducts( string url )
    {
        var result = await Http.GetFromJsonAsync<ApiReply<ProductSearchResponse?>>( url );

        if ( result == null )
        {
            ProductSearchChanged?.Invoke( new ApiReply<ProductSearchResponse?>( null, false, SERVER_RESPONSE_MESSAGE_NULL ) );
            return;
        }

        if ( result.Data == null || !result.Success )
        {
            ProductSearchChanged?.Invoke( new ApiReply<ProductSearchResponse?>( null, false, result.Message ?? SERVER_RESPONSE_MESSAGE_FAILURE_NO_MESSAGE ) );
            return;
        }

        ProductSearchChanged?.Invoke( result );
    }
}