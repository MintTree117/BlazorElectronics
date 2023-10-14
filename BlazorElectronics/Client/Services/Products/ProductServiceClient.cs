using System.Net.Http.Json;
using System.Text;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DataTransferObjects.Products;

namespace BlazorElectronics.Client.Services.Products;

public class ProductServiceClient : IProductServiceClient
{
    public event Action<ServiceResponse<ProductSearchResults_DTO?>?>? ProductSearchChanged;

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

    public async Task<ServiceResponse<ProductSearchSuggestions_DTO?>?> GetProductSearchSuggestions( string searchText )
    {
        string url = $"{PRODUCT_SEARCH_SUGGESTIONS_URL}{searchText}";
        return await _http.GetFromJsonAsync<ServiceResponse<ProductSearchSuggestions_DTO?>>( url );
    }

    public void UpdateProductSearchPage( int page )
    {
        SearchRequest ??= new ProductSearchRequest_DTO();
        SearchRequest.Page = page;
    }
    public void UpdateProductSearchResultsCount( int count )
    {
        SearchRequest ??= new ProductSearchRequest_DTO();
        SearchRequest.Rows = count;
    }
    public string GetProductSearchUrl()
    {
        SearchRequest ??= new ProductSearchRequest_DTO();
        
        var urlBuilder = new StringBuilder();

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

    public async Task<ServiceResponse<ProductsFeatured_DTO?>?> GetFeaturedProducts()
    {
        var result = await _http.GetFromJsonAsync<ServiceResponse<ProductsFeatured_DTO?>>( "api/Product/featured" );
        return result;
    }
    public async Task SearchProductsByCategory( string categoryUrl, ProductSearchRequest_DTO? filters )
    {
        string queryString = $"?filters={Uri.EscapeDataString( Newtonsoft.Json.JsonConvert.SerializeObject( filters ) )}";
        string url = $"{PRODUCT_SEARCH_CATEGORY_URL}/{categoryUrl}{queryString}";
        await SearchProducts( url );
    }
    public async Task SearchProductsByText( string searchText, ProductSearchRequest_DTO? filters )
    {
        string queryString = $"?filters={Uri.EscapeDataString( Newtonsoft.Json.JsonConvert.SerializeObject( filters ) )}";
        string url = $"{PRODUCT_SEARCH_TEXT_URL}/{searchText}{queryString}";
        await SearchProducts( url );
    }
    public async Task SearchProductsByCategoryAndText( string categoryUrl, string searchText, ProductSearchRequest_DTO? filters )
    {
        string queryString = $"?filters={Uri.EscapeDataString( Newtonsoft.Json.JsonConvert.SerializeObject( filters ) )}";
        string url = $"{PRODUCT_SEARCH_CATEGORY_TEXT_URL}/{categoryUrl}/{searchText}{queryString}";
        await SearchProducts( url );
    }
    public async Task<ServiceResponse<ProductDetails_DTO?>> GetProductDetails( int productId )
    {
        string url = $"{PRODUCT_DETAILS_URL}/{productId}";
        var serverResponse = await _http.GetFromJsonAsync<ServiceResponse<ProductDetails_DTO?>>( url );
        return serverResponse ?? new ServiceResponse<ProductDetails_DTO?>( null, false, SERVER_RESPONSE_MESSAGE_NULL );
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