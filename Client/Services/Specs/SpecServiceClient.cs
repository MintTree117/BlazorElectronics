using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Specs;

namespace BlazorElectronics.Client.Services.Specs;

public sealed class SpecServiceClient : CachedClientService<LookupSpecsDto>, ISpecServiceClient
{
    public SpecServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage, 1, "Spec Lookups" ) { }

    public async Task<ServiceReply<LookupSpecsDto?>> GetSpecLookups()
    {
        LookupSpecsDto? cached = await TryGetCachedItem();
        
        if ( cached is not null )
            return new ServiceReply<LookupSpecsDto?>( cached );
        
        ServiceReply<LookupSpecsDto?> reply = await TryGetRequest<LookupSpecsDto?>( "api/specs/get" );

        if ( !reply.Success || reply.Data is null )
            return new ServiceReply<LookupSpecsDto?>( reply.ErrorType, reply.Message );

        await TrySetCachedItem( reply.Data );
        return reply;
    }
}