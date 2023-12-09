using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Specs;

namespace BlazorElectronics.Client.Services.Specs;

public sealed class SpecServiceClient : ClientService, ISpecServiceClient
{
    public SpecServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }

    SpecsResponse? _lookups;
    
    public async Task<ServiceReply<SpecsResponse?>> GetSpecLookups()
    {
        if ( _lookups is not null )
            return new ServiceReply<SpecsResponse?>( _lookups );
        
        ServiceReply<SpecsResponse?> reply = await TryGetRequest<SpecsResponse?>( "api/specs/get" );
        _lookups = reply.Data;

        return reply;
    }
}