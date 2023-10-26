using System.Net.Http.Json;
using System.Text;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DtosInbound.Products;
using BlazorElectronics.Shared.DtosOutbound.Products;

namespace BlazorElectronics.Client.Services.Products;

public class ProductServiceClient : IProductServiceClient
{
    public event Action<string>? ExceptionEvent;
    public event Action<ServiceResponse<ProductSearchResults_DTO?>?>? ProductSearchChanged;
    public event Action<string>? ProductSearchNullabillityTest;

    public ProductSearchRequest_DTO? SearchRequest { get; set; }

    const string SERVER_RESPONSE_MESSAGE_NULL = "Server returned null!";
    const string SERVER_RESPONSE_MESSAGE_FAILURE_NO_MESSAGE = "Server returned null data without a return message!";
    const string PRODUCT_SEARCH_SUGGESTIONS_URL = "api/Product/search-suggestions/";
    const string PRODUCT_SEARCH_CATEGORY_TEXT_URL = "api/Product/search-category-text/";
    const string PRODUCT_SEARCH_CATEGORY_URL = "api/Product/search-category";
    const string PRODUCT_SEARCH_TEXT_URL = "api/Product/search-text";
    const string PRODUCT_DETAILS_URL = "api/Product/details";

    readonly HttpClient _http;

    public ProductServiceClient( HttpClient http )
    {
        _http = http;
    }
    
    public void ClearSearchRequest()
    {
        SearchRequest = null;
    }
    public void UpdateSearchCategory( string url )
    {
        SearchRequest ??= new ProductSearchRequest_DTO();
        SearchRequest.CategoryUrl = url;

        if ( SearchRequest == null )
            ProductSearchNullabillityTest?.Invoke( "On Category" );
    }
    public void UpdateSearchText( string text )
    {
        SearchRequest ??= new ProductSearchRequest_DTO();
        SearchRequest.SearchText = text;

        if ( SearchRequest == null )
            ProductSearchNullabillityTest?.Invoke( "On Text" );
    }
    public void UpdateSearchPage( int page )
    {
        SearchRequest ??= new ProductSearchRequest_DTO();
        SearchRequest.Page = page;

        if ( SearchRequest == null )
            ProductSearchNullabillityTest?.Invoke( "On Page" );
    }
    public void UpdateSearchResultsCount( int count )
    {
        SearchRequest ??= new ProductSearchRequest_DTO();
        SearchRequest.NumberOfResults = count;

        if ( SearchRequest == null )
            ProductSearchNullabillityTest?.Invoke( "On Count" );
    }
    public string? GetProductSearchUrl()
    {
        if ( SearchRequest == null )
            ProductSearchNullabillityTest?.Invoke( "On Search" );
        
        SearchRequest ??= new ProductSearchRequest_DTO();
        
        var urlBuilder = new StringBuilder();

        if ( string.IsNullOrEmpty( SearchRequest.CategoryUrl ) && string.IsNullOrEmpty( SearchRequest.SearchText ) )
            return null;

        if ( !string.IsNullOrEmpty( SearchRequest.CategoryUrl ) )
        {
            urlBuilder.Append( $"/{SearchRequest.CategoryUrl}/" );

            if ( !string.IsNullOrEmpty( SearchRequest.SearchText ) )
                urlBuilder.Append( $"{SearchRequest.SearchText}/" );
        }
        else if ( !string.IsNullOrEmpty( SearchRequest.SearchText ) )
        {
            urlBuilder.Append( $"/{SearchRequest.SearchText}/" );
        }
        
        urlBuilder.Append( $"?page={SearchRequest.Page}" );
        urlBuilder.Append( $"&rows={SearchRequest.NumberOfResults}" );

        if ( SearchRequest.MinPrice != null )
            urlBuilder.Append( $"&minPrice={SearchRequest.MinPrice.Value}" );
        if ( SearchRequest.MaxPrice != null )
            urlBuilder.Append( $"&maxPrice={SearchRequest.MaxPrice.Value}" );
        if ( SearchRequest.MinRating != null )
            urlBuilder.Append( $"&minRating={SearchRequest.MinRating.Value}" );
        if ( SearchRequest.MaxRating != null )
            urlBuilder.Append( $"&maxRating={SearchRequest.MaxRating.Value}" );

        if ( SearchRequest.SpecFilters == null )
            return urlBuilder.ToString();

        foreach ( ProductSpecFilter_DTO specFilter in SearchRequest.SpecFilters )
        {
            if ( string.IsNullOrEmpty( specFilter.SpecName ) || specFilter.SpecValue == null )
                continue;
            urlBuilder.Append( $"&{specFilter.SpecName}={specFilter.SpecValue}" );
        }
        
        return urlBuilder.ToString();
    }
    
    public async Task<ServiceResponse<ProductSearchSuggestions_DTO?>> GetProductSearchSuggestions( string searchText )
    {
        string url = $"{PRODUCT_SEARCH_SUGGESTIONS_URL}{searchText}";

        try
        {
            var response = await _http.GetFromJsonAsync<ServiceResponse<ProductSearchSuggestions_DTO?>>( url );

            if ( response == null )
                return new ServiceResponse<ProductSearchSuggestions_DTO?>( null, false, "Service response is null!" );

            return !response.Success
                ? new ServiceResponse<ProductSearchSuggestions_DTO?>( null, false, response.Message ??= "Failed to retrieve Search Suggestions; message is null!" )
                : response;
        }
        catch ( Exception e )
        {
            return new ServiceResponse<ProductSearchSuggestions_DTO?>( null, false, e.Message );
        }
    }
    public async Task SearchProductsByCategory( string categoryUrl, ProductSearchRequest_DTO? filters )
    {
        try
        {
            string queryString = $"?filters={Uri.EscapeDataString( Newtonsoft.Json.JsonConvert.SerializeObject( filters ) )}";
            string url = $"{PRODUCT_SEARCH_CATEGORY_URL}/{categoryUrl}{queryString}";
            await SearchProducts( url );
        }
        catch ( Exception e )
        {
            ExceptionEvent?.Invoke( e.Message );
        }
    }
    public async Task SearchProductsByText( string searchText, ProductSearchRequest_DTO? filters )
    {
        try
        {
            string queryString = $"?filters={Uri.EscapeDataString( Newtonsoft.Json.JsonConvert.SerializeObject( filters ) )}";
            string url = $"{PRODUCT_SEARCH_TEXT_URL}/{searchText}{queryString}";
            await SearchProducts( url );   
        }
        catch ( Exception e )
        {
            ExceptionEvent?.Invoke( e.Message );
        }
    }
    public async Task SearchProductsByCategoryAndText( string categoryUrl, string searchText, ProductSearchRequest_DTO? filters )
    {
        try
        {
            string queryString = $"?filters={Uri.EscapeDataString( Newtonsoft.Json.JsonConvert.SerializeObject( filters ) )}";
            string url = $"{PRODUCT_SEARCH_CATEGORY_TEXT_URL}/{categoryUrl}/{searchText}{queryString}";
            await SearchProducts( url );   
        }
        catch ( Exception e )
        {
            ExceptionEvent?.Invoke( e.Message );
        }
    }
    public async Task<ServiceResponse<ProductDetails_DTO?>> GetProductDetails( int productId )
    {
        try
        {
            string url = $"{PRODUCT_DETAILS_URL}/{productId}";
            var response = await _http.GetFromJsonAsync<ServiceResponse<ProductDetails_DTO?>>( url );

            if ( response == null )
                return new ServiceResponse<ProductDetails_DTO?>( null, false, "Service response is null!" );

            return !response.Success
                ? new ServiceResponse<ProductDetails_DTO?>( null, false, response.Message ??= "Failed to retrieve Product Details; message is null!" )
                : response;
        }
        catch ( Exception e )
        {
            return new ServiceResponse<ProductDetails_DTO?>( null, false, e.Message );
        }
    }
    
    async Task SearchProducts( string url )
    {
        var result = await _http.GetFromJsonAsync<ServiceResponse<ProductSearchResults_DTO?>>( url );

        if ( result == null )
        {
            ProductSearchChanged?.Invoke( new ServiceResponse<ProductSearchResults_DTO?>( null, false, SERVER_RESPONSE_MESSAGE_NULL ) );
            return;
        }

        if ( result.Data == null || !result.Success )
        {
            ProductSearchChanged?.Invoke( new ServiceResponse<ProductSearchResults_DTO?>( null, false, result.Message ?? SERVER_RESPONSE_MESSAGE_FAILURE_NO_MESSAGE ) );
            return;
        }

        ProductSearchChanged?.Invoke( result );
    }
}