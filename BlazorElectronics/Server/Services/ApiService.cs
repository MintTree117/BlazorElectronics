using BlazorElectronics.Server.Models.SpecLookups;

namespace BlazorElectronics.Server.Services;

public abstract class ApiService
{
    protected const string NO_DATA_FOUND_MESSAGE = "No data was found in cache or database!";
    protected const string INTERNAL_SERVER_ERROR_MESSAGE = "An internal server error occured!";
    
    protected readonly ILogger<ApiService> Logger;
    
    public ApiService( ILogger<ApiService> logger )
    {
        Logger = logger;
    }
    
    protected static string ConvertSpecValuesToString( IEnumerable<SpecLookupValueModel> values )
    {
        List<string> specValues = values
            .OrderBy( spec => spec.SpecValueId )
            .Select( spec => spec.SpecValue )
            .ToList();

        return string.Join( ",", specValues );
    }
}