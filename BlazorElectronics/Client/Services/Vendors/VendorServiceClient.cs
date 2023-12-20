using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Vendors;

namespace BlazorElectronics.Client.Services.Vendors;

public sealed class VendorServiceClient : CachedClientService<VendorsDto>, IVendorServiceClient
{
    public VendorServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage, 1, "Vendors" ) { }
    
    
    public async Task<ServiceReply<VendorsDto?>> GetVendors()
    {
        VendorsDto? cached = await TryGetCachedItem();
        
        if ( cached is not null )
            return new ServiceReply<VendorsDto?>( cached );

        ServiceReply<VendorsDto?> reply = await TryGetRequest<VendorsDto?>( "api/vendors/get" );

        if ( !reply.Success || reply.Data is null )
            return new ServiceReply<VendorsDto?>( reply.ErrorType, reply.Message );
        
        await TrySetCachedItem( reply.Data );
        return reply;
    }
}