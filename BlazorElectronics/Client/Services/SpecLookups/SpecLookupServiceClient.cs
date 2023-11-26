using Blazored.LocalStorage;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.SpecLookups;

namespace BlazorElectronics.Client.Services.SpecLookups;

public sealed class SpecLookupServiceClient : ClientService, ISpecLookupServiceClient
{
    Dictionary<int, SpecLookupResponse> _responsesBySpecId;
    Dictionary<PrimaryCategory, List<int>> _responseIdsByCategory;

    public SpecLookupServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }
}