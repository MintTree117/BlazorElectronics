using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Server.Core.Models.Promos;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.Promos;

namespace BlazorElectronics.Server.Core.Services;

public sealed class PromoService : _ApiService, IPromoService
{
    readonly IPromoRepository _repository;
    
    public PromoService( ILogger<PromoService> logger, IPromoRepository repository )
        : base( logger )
    {
        _repository = repository;
    }
    
    public async Task<ServiceReply<List<PromoEditDto>?>> GetView()
    {
        try
        {
            IEnumerable<PromoModel>? models = await _repository.GetView();
            List<PromoEditDto>? dtos = MapView( models );

            return dtos is not null
                ? new ServiceReply<List<PromoEditDto>?>( dtos )
                : new ServiceReply<List<PromoEditDto>?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<List<PromoEditDto>?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<PromoEditDto?>> GetEdit( int promoId )
    {
        try
        {
            PromoModel? model = await _repository.GetEdit( promoId );
            PromoEditDto? dto = MapEdit( model );

            return dto is not null
                ? new ServiceReply<PromoEditDto?>( dto )
                : new ServiceReply<PromoEditDto?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<PromoEditDto?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<int>> Add( PromoEditDto dto )
    {
        try
        {
            int promoId = await _repository.Insert( MapModel( dto ) );

            return promoId > 0
                ? new ServiceReply<int>( promoId )
                : new ServiceReply<int>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<int>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<bool>> Update( PromoEditDto dto )
    {
        try
        {
            bool success = await _repository.Update( MapModel( dto ) );

            return success
                ? new ServiceReply<bool>( true )
                : new ServiceReply<bool>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<bool>> Remove( int promoId )
    {
        try
        {
            bool success = await _repository.Delete( promoId);

            return success
                ? new ServiceReply<bool>( true )
                : new ServiceReply<bool>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
    }

    static PromoModel MapModel( PromoEditDto dto )
    {
        return new PromoModel
        {
            PromoId = dto.Id,
            PromoCode = dto.Name,
            PromoDiscount = dto.PromoDiscount
        };
    }
    static List<PromoEditDto>? MapView( IEnumerable<PromoModel>? models )
    {
        if ( models is null )
            return null;
        
        List<PromoEditDto> dtos = new();

        foreach ( PromoModel m in models )
        {
            PromoEditDto? edit = MapEdit( m );
            if ( edit is not null )
                dtos.Add( edit );
        }

        return dtos;
    }
    static PromoEditDto? MapEdit( PromoModel? model )
    {
        if ( model is null )
            return null;
        
        return new PromoEditDto
        {
            Id = model.PromoId,
            Name = model.PromoCode,
            PromoDiscount = model.PromoDiscount
        };
    }
}