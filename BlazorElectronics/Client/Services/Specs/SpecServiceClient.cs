using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Specs;

namespace BlazorElectronics.Client.Services.Specs;

public sealed class SpecServiceClient : ClientService, ISpecServiceClient
{
    public SpecServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }

    LookupSpecsDto? _lookups;
    
    public async Task<ServiceReply<LookupSpecsDto?>> GetSpecLookups()
    {
        if ( _lookups is not null )
            return new ServiceReply<LookupSpecsDto?>( _lookups );
        
        ServiceReply<LookupSpecsDto?> reply = await TryGetRequest<LookupSpecsDto?>( "api/specs/get" );
        _lookups = reply.Data;

        return reply;
    }
}