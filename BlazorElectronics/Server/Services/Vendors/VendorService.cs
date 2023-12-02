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
    
    public async Task<ServiceReply<VendorsResponse?>> GetVendors()
    {
        if ( _cachedVendors is not null && _cachedVendors.IsValid( MAX_VENDOR_LIFE ) )
            return new ServiceReply<VendorsResponse?>( _cachedVendors.Object );

        VendorsModel? model;

        try
        {
            model = await _repository.Get();
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<VendorsResponse?>( ServiceErrorType.ServerError );
        }

        VendorsResponse? response = MapResponse( model );

        if ( response is null )
            return new ServiceReply<VendorsResponse?>( ServiceErrorType.NotFound );

        _cachedVendors = new CachedObject<VendorsResponse>( response );
        return new ServiceReply<VendorsResponse?>( response );
    }
    public async Task<ServiceReply<List<AdminItemViewDto>?>> GetView()
    {
        VendorsModel? model;

        try
        {
            model = await _repository.Get();
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<List<AdminItemViewDto>?>( ServiceErrorType.ServerError );
        }

        List<AdminItemViewDto>? view = MapView( model );

        return view is not null
            ? new ServiceReply<List<AdminItemViewDto>?>( view )
            : new ServiceReply<List<AdminItemViewDto>?>( ServiceErrorType.NotFound );
    }
    public async Task<ServiceReply<VendorEditDto?>> GetEdit( int vendorId )
    {
        VendorEditModel? model;

        try
        {
            model = await _repository.GetEdit( vendorId );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<VendorEditDto?>( ServiceErrorType.ServerError );
        }

        VendorEditDto? dto = MapEdit( model );

        return dto is not null
            ? new ServiceReply<VendorEditDto?>( dto )
            : new ServiceReply<VendorEditDto?>( ServiceErrorType.NotFound );
    }
    public async Task<ServiceReply<int>> Add( VendorEditDto dto )
    {
        try
        {
            int result = await _repository.Insert( dto );

            return result > 0
                ? new ServiceReply<int>( result )
                : new ServiceReply<int>( ServiceErrorType.NotFound );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<int>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<bool>> Update( VendorEditDto dto )
    {
        try
        {
            bool result = await _repository.Update( dto );

            return result
                ? new ServiceReply<bool>( result )
                : new ServiceReply<bool>( ServiceErrorType.NotFound );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<bool>> Remove( int vendorId )
    {
        try
        {
            bool result = await _repository.Delete( vendorId );

            return result
                ? new ServiceReply<bool>( result )
                : new ServiceReply<bool>( ServiceErrorType.NotFound );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
    }

    static VendorsResponse? MapResponse( VendorsModel? model )
    {
        if ( model?.Vendors is null || model.Categories is null )
            return null;
        
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

        return response;
    }
    static List<AdminItemViewDto>? MapView( VendorsModel? model )
    {
        return model?.Vendors?
            .Select( vendor => new AdminItemViewDto { Id = vendor.VendorId, Name = vendor.VendorName } )
            .ToList();
    }
    static VendorEditDto? MapEdit( VendorEditModel? model )
    {
        if ( model?.Vendor is null )
            return null;
        
        List<int>? categoryIds = model.Categories?
            .Select( c => c.PrimaryCategoryId )
            .ToList();

        return new VendorEditDto
        {
            VendorId = model.Vendor.VendorId,
            VendorName = model.Vendor.VendorName,
            VendorUrl = model.Vendor.VendorUrl,
            PrimaryCategories = ConvertPrimaryCategoriesToString( categoryIds ?? new List<int>() )
        };
    }
}