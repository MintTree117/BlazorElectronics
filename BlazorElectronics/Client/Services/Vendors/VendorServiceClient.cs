using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Vendors;

namespace BlazorElectronics.Client.Services.Vendors;

public sealed class VendorServiceClient : ClientService, IVendorServiceClient
{
    public VendorServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }

    VendorsResponse? _vendors;
    
    public async Task<ServiceReply<VendorsResponse?>> GetVendors()
    {
        if ( _vendors is not null )
            return new ServiceReply<VendorsResponse?>( _vendors );

        ServiceReply<VendorsResponse?> reply = await TryGetRequest<VendorsResponse?>( "api/vendors/get" );
        _vendors = reply.Data;

        return reply;
    }
}