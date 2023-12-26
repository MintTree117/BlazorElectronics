using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Server.Core.Models.Vendors;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.Vendors;

namespace BlazorElectronics.Server.Core.Services;

public sealed class VendorService : _CachedApiService, IVendorService
{
    readonly IVendorRepository _repository;
    VendorsDto? _cachedVendors;

    public VendorService( ILogger<_ApiService> logger, IVendorRepository repository )
        : base( logger, repository, 4, "Vendors" )
    {
        _repository = repository;
    }
    
    protected override void UpdateCache()
    {
        try
        {
            VendorsModel? models = Task
                .Run( () => _repository.Get() ).Result;

            if ( models is null )
                return;

            MapModels( models );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
        }
    }
    
    public async Task<ServiceReply<VendorsDto?>> GetVendors()
    {
        if ( _cachedVendors is not null )
            return new ServiceReply<VendorsDto?>( _cachedVendors );

        try
        {
            VendorsModel? model = await _repository.Get();
            VendorsDto? response = MapModels( model );
            _cachedVendors = response;
            
            return response is not null
                ? new ServiceReply<VendorsDto?>( response ) 
                : new ServiceReply<VendorsDto?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<VendorsDto?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<List<CrudViewDto>?>> GetView()
    {
        try
        {
            IEnumerable<VendorDto>? models = await _repository.GetView();
            List<CrudViewDto>? view = MapView( models );

            return view is not null
                ? new ServiceReply<List<CrudViewDto>?>( view )
                : new ServiceReply<List<CrudViewDto>?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<List<CrudViewDto>?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<VendorEditDtoDto?>> GetEdit( int vendorId )
    {
        try
        {
            VendorEditModel? model = await _repository.GetEdit( vendorId );
            VendorEditDtoDto? dto = MapEdit( model );

            return dto is not null
                ? new ServiceReply<VendorEditDtoDto?>( dto )
                : new ServiceReply<VendorEditDtoDto?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<VendorEditDtoDto?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<int>> Add( VendorEditDtoDto dtoDto )
    {
        try
        {
            int id = await _repository.Insert( dtoDto );
            
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
    public async Task<ServiceReply<bool>> Update( VendorEditDtoDto dtoDto )
    {
        try
        {
            bool result = await _repository.Update( dtoDto );

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

    static VendorsDto? MapModels( VendorsModel? model )
    {
        if ( model?.Vendors is null || model.Categories is null )
            return null;
        
        var response = new VendorsDto();
        
        foreach ( VendorDto v in model.Vendors )
        {
            response.VendorsById.TryAdd( v.VendorId, v );
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
    static List<CrudViewDto>? MapView( IEnumerable<VendorDto>? models )
    {
        return models?
            .Select( vendor => new CrudViewDto { Id = vendor.VendorId, Name = vendor.VendorName } )
            .ToList();
    }
    static VendorEditDtoDto? MapEdit( VendorEditModel? model )
    {
        if ( model?.Vendor is null )
            return null;
        
        List<int> categories = model.Categories is not null
            ? model.Categories.Select( c => c.PrimaryCategoryId ).ToList()
            : new List<int>();

        return new VendorEditDtoDto
        {
            VendorId = model.Vendor.VendorId,
            VendorName = model.Vendor.VendorName,
            VendorUrl = model.Vendor.VendorUrl,
            PrimaryCategories = categories
        };
    }
}