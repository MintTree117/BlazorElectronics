using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Vendors;

namespace BlazorElectronics.Client.Services.Vendors;

public sealed class VendorServiceClient : ClientService, IVendorServiceClient
{
    public VendorServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }

    VendorsDto? _vendors;
    
    public async Task<ServiceReply<VendorsDto?>> GetVendors()
    {
        if ( _vendors is not null )
            return new ServiceReply<VendorsDto?>( _vendors );

        ServiceReply<VendorsDto?> reply = await TryGetRequest<VendorsDto?>( "api/vendors/get" );
        _vendors = reply.Data;

        return reply;
    }
}