using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.SpecLookups;

namespace BlazorElectronics.Client.Services.SpecLookups;

public sealed class SpecLookupServiceClient : ClientService, ISpecLookupServiceClient
{
    const string API_ROUTE = "api/speclookups/get";
    SpecsResponse? _lookups;
    
    public SpecLookupServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }
    
    public async Task<ServiceReply<SpecsResponse?>> GetSpecLookups()
    {
        ServiceReply<SpecsResponse?> reply = await TryGetRequest<SpecsResponse?>( API_ROUTE );
        _lookups = reply.Data;

        return _lookups is not null
            ? new ServiceReply<SpecsResponse?>( _lookups )
            : new ServiceReply<SpecsResponse?>( reply.ErrorType, reply.Message );
    }
}