using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Server.Core.Models;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Server.Core.Services;

public sealed class CategoryService : ApiService, ICategoryService
{
    const int CACHE_LIFE = 4;
    const string INVALID_CATEGORY_MESSAGE = "Invalid Category!";
    readonly ICategoryRepository _repository;

    CachedObject<CategoryData>? _cachedData;
    CachedObject<List<CategoryLightDto>>? _cachedResponse;

    public CategoryService( ILogger<ApiService> logger, ICategoryRepository repository ) : base( logger )
    {
        _repository = repository;
    }

    public async Task<ServiceReply<List<int>?>> GetPrimaryCategoryIds()
    {
        ServiceReply<bool> getReply = await TryGetData();

        return getReply.Success && _cachedData is not null
            ? new ServiceReply<List<int>?>( _cachedData.Object.PrimaryIds )
            : new ServiceReply<List<int>?>( getReply.ErrorType, getReply.Message );
    }
    public async Task<ServiceReply<CategoryData?>> GetCategoryData()
    {
        ServiceReply<bool> getReply = await TryGetData();

        return getReply.Success && _cachedData is not null
            ? new ServiceReply<CategoryData?>( _cachedData.Object )
            : new ServiceReply<CategoryData?>( getReply.ErrorType, getReply.Message );
    }
    public async Task<ServiceReply<List<CategoryLightDto>?>> GetCategoryResponse()
    {
        ServiceReply<bool> getReply = await TryGetData();

        return getReply.Success && _cachedResponse is not null
            ? new ServiceReply<List<CategoryLightDto>?>( _cachedResponse.Object )
            : new ServiceReply<List<CategoryLightDto>?>( getReply.ErrorType, getReply.Message );
    }
    public async Task<ServiceReply<int>> ValidateCategoryUrl( string url )
    {
        ServiceReply<bool> reply = await TryGetData();

        if ( !reply.Success || _cachedData is null )
            return new ServiceReply<int>( reply.ErrorType, reply.Message );

        return _cachedData.Object.ValidateUrl( url, out int categoryId )
            ? new ServiceReply<int>( categoryId )
            : new ServiceReply<int>( ServiceErrorType.ValidationError, INVALID_CATEGORY_MESSAGE );
    }
    public async Task<ServiceReply<List<CategoryViewDtoDto>?>> GetCategoriesView()
    {
        try
        {
            IEnumerable<CategoryFullDto>? result = await _repository.Get();
            List<CategoryViewDtoDto>? dto = await MapView( result );

            return dto is not null
                ? new ServiceReply<List<CategoryViewDtoDto>?>( dto )
                : new ServiceReply<List<CategoryViewDtoDto>?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<List<CategoryViewDtoDto>?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<CategoryEditDto?>> GetCategoryEdit( int categoryId )
    {
        try
        {
            CategoryFullDto? model = await _repository.GetEdit( categoryId );
            CategoryEditDto? dto = MapEdit( model );

            return dto is not null
                ? new ServiceReply<CategoryEditDto?>( dto )
                : new ServiceReply<CategoryEditDto?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<CategoryEditDto?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<bool>> AddBulkCategories( List<CategoryEditDto> categories )
    {
        try
        {
            bool result = await _repository.BulkInsert( categories );

            return result
                ? new ServiceReply<bool>( true )
                : new ServiceReply<bool>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<int>> AddCategory( CategoryEditDto dto )
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
    public async Task<ServiceReply<bool>> UpdateCategory( CategoryEditDto dto )
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
    public async Task<ServiceReply<bool>> RemoveCategory( int categoryId )
    {
        try
        {
            bool result = await _repository.Delete( categoryId );

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

    async Task<ServiceReply<bool>> TryGetData()
    {
        if ( _cachedData is not null && _cachedData.IsValid( CACHE_LIFE ) )
            return new ServiceReply<bool>( true );

        IEnumerable<CategoryFullDto>? models;

        try
        {
            models = await _repository.Get();
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
        
        if ( models is null )
            return new ServiceReply<bool>( ServiceErrorType.NotFound );
        
        await MapModels( models );
        return new ServiceReply<bool>( true );
    }
    
    async Task MapModels( IEnumerable<CategoryFullDto> models )
    {
        await Task.Run( () =>
        {
            List<CategoryFullDto> list = models.ToList();
            List<CategoryLightDto> response = MapResponse( list );
            CategoryData data = new ( list );
            _cachedData = new CachedObject<CategoryData>( data );
            _cachedResponse = new CachedObject<List<CategoryLightDto>>( response );
        } );
    }
    List<CategoryLightDto> MapResponse( IEnumerable<CategoryFullDto> models )
    {
        return models
            .Select( m => new CategoryLightDto
            {
                ParentId = m.ParentCategoryId,
                Id = m.CategoryId,
                Tier = m.Tier,
                ImageUrl = m.ImageUrl,
                Name = m.Name,
                Url = m.ApiUrl
            } )
            .ToList();
    }
    static async Task<List<CategoryViewDtoDto>?> MapView( IEnumerable<CategoryFullDto>? models )
    {
        if ( models is null )
            return null;

        return await Task.Run( () =>
        {
            return models
                .Select( m => new CategoryViewDtoDto { Id = m.CategoryId, ParentCategoryId = m.ParentCategoryId, Tier = m.Tier, Name = m.Name } )
                .ToList();
        } );
    }
    static CategoryEditDto? MapEdit( CategoryFullDto? model )
    {
        if ( model is null )
            return null;

        return new CategoryEditDto
        {
            CategoryId = model.CategoryId,
            ParentCategoryId = model.ParentCategoryId,
            Tier = model.Tier,
            Name = model.Name,
            ApiUrl = model.ApiUrl,
            ImageUrl = model.ImageUrl
        };
    }
}