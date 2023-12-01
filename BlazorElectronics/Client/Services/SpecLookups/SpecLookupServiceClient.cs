using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.SpecLookups;

namespace BlazorElectronics.Client.Services.SpecLookups;

public sealed class SpecLookupServiceClient : ClientService, ISpecLookupServiceClient
{
    const string API_ROUTE = "api/speclookups/get";
    SpecLookupsResponse? _lookups;
    
    public SpecLookupServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }
    
    public async Task<ServiceReply<SpecLookupsResponse?>> GetSpecLookups()
    {
        ServiceReply<SpecLookupsResponse?> reply = await TryGetRequest<SpecLookupsResponse?>( API_ROUTE );
        _lookups = reply.Data;

        return _lookups is not null
            ? new ServiceReply<SpecLookupsResponse?>( _lookups )
            : new ServiceReply<SpecLookupsResponse?>( reply.ErrorType, reply.Message );
    }
}