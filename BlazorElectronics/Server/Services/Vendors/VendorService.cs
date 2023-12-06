using BlazorElectronics.Server.Dtos;
using BlazorElectronics.Server.Models.Vendors;
using BlazorElectronics.Server.Repositories;
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

        try
        {
            VendorsModel? model = await _repository.Get();
            VendorsResponse? response = MapResponse( model );
            _cachedVendors = response is not null ? new CachedObject<VendorsResponse>( response ) : null;
            
            return response is not null 
                ? new ServiceReply<VendorsResponse?>( response ) 
                : new ServiceReply<VendorsResponse?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<VendorsResponse?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<List<CrudView>?>> GetView()
    {
        try
        {
            IEnumerable<VendorModel>? models = await _repository.GetView();
            List<CrudView>? view = MapView( models );

            return view is not null
                ? new ServiceReply<List<CrudView>?>( view )
                : new ServiceReply<List<CrudView>?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<List<CrudView>?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<VendorEdit?>> GetEdit( int vendorId )
    {
        try
        {
            VendorEditModel? model = await _repository.GetEdit( vendorId );
            VendorEdit? dto = MapEdit( model );

            return dto is not null
                ? new ServiceReply<VendorEdit?>( dto )
                : new ServiceReply<VendorEdit?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<VendorEdit?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<int>> Add( VendorEdit dto )
    {
        try
        {
            int id = await _repository.Insert( dto );
            
            return id > 0
                ? new ServiceReply<int>( id )
                : new ServiceReply<int>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<int>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<bool>> Update( VendorEdit dto )
    {
        try
        {
            bool result = await _repository.Update( dto );

            return result
                ? new ServiceReply<bool>( result )
                : new ServiceReply<bool>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
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
        catch ( RepositoryException e )
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
            response.VendorsById.TryAdd( v.VendorId, new Vendor
            {
                VendorId = v.VendorId,
                VendorName = v.VendorName,
                VendorUrl = v.VendorUrl
            } );
        }

        foreach ( VendorCategoryModel c in model.Categories )
        {
            if ( !response.VendorIdsByCategory.TryGetValue( c.PrimaryCategoryId, out List<int>? ids ) )
            {
                ids = new List<int>();
                response.VendorIdsByCategory.Add( c.PrimaryCategoryId, ids );
            }

            ids.Add( c.VendorId );
        }

        return response;
    }
    static List<CrudView>? MapView( IEnumerable<VendorModel>? models )
    {
        return models?
            .Select( vendor => new CrudView { Id = vendor.VendorId, Name = vendor.VendorName } )
            .ToList();
    }
    static VendorEdit? MapEdit( VendorEditModel? model )
    {
        if ( model?.Vendor is null )
            return null;
        
        List<int> categories = model.Categories is not null
            ? model.Categories.Select( c => c.PrimaryCategoryId ).ToList()
            : new List<int>();

        return new VendorEdit
        {
            VendorId = model.Vendor.VendorId,
            VendorName = model.Vendor.VendorName,
            VendorUrl = model.Vendor.VendorUrl,
            PrimaryCategories = categories
        };
    }
}