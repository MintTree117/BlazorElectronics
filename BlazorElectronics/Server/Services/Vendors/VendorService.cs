using BlazorElectronics.Server.Dtos;
using BlazorElectronics.Server.Models.Vendors;
using BlazorElectronics.Server.Repositories.Vendors;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.Vendors;

namespace BlazorElectronics.Server.Services.Vendors;

public sealed class VendorService : ApiService, IVendorService
{
    const int MAX_VENDOR_LIFE = 8;

    readonly IVendorRepository _repository;
    CachedObject<VendorsResponse>? _cachedVendors;

    public VendorService( ILogger<ApiService> logger, IVendorRepository repository )
        : base( logger )
    {
        _repository = repository;
    }
    
    public async Task<ApiReply<VendorsResponse?>> GetVendors()
    {
        if ( _cachedVendors is not null && _cachedVendors.IsValid( MAX_VENDOR_LIFE ) )
        {
            return new ApiReply<VendorsResponse?>( _cachedVendors.Object );
        }

        VendorsModel? model;

        try
        {
            model = await _repository.Get();
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ApiReply<VendorsResponse?>( INTERNAL_SERVER_ERROR_MESSAGE );
        }

        if ( model is null || !MapModel( model ) )
            return new ApiReply<VendorsResponse?>( NO_DATA_FOUND_MESSAGE );

        return new ApiReply<VendorsResponse?>( _cachedVendors!.Object );
    }

    bool MapModel( VendorsModel model )
    {
        if ( model.Vendors is null || model.Categories is null )
            return false;
        
        var response = new VendorsResponse();

        foreach ( VendorModel v in model.Vendors )
        {
            response.VendorsById.TryAdd( v.VendorId, new VendorDto
            {
                VendorId = v.VendorId,
                VendorName = v.VendorName,
                VendorUrl = v.VendorUrl
            } );
        }

        foreach ( VendorCategoryModel c in model.Categories )
        {
            var pc = ( PrimaryCategory ) c.PrimaryCategoryId;

            if ( !response.VendorIdsByCategory.TryGetValue( pc, out List<int>? ids ) )
            {
                ids = new List<int>();
                response.VendorIdsByCategory.Add( pc, ids );
            }

            ids.Add( c.PrimaryCategoryId );
        }

        _cachedVendors = new CachedObject<VendorsResponse>( response );
        return true;
    }
}