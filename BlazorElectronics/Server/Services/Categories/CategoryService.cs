using BlazorElectronics.Server.Dtos;
using BlazorElectronics.Server.Repositories;
using BlazorElectronics.Server.Repositories.Categories;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Server.Services.Categories;

public class CategoryService : ApiService, ICategoryService
{
    const int CACHE_LIFE = 4;
    const string INVALID_CATEGORY_MESSAGE = "Invalid Category!";
    readonly ICategoryRepository _repository;

    CachedObject<CategoryData>? _cachedData;
    CachedObject<List<CategoryResponse>>? _cachedResponse;

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
    public async Task<ServiceReply<List<CategoryResponse>?>> GetCategoryResponse()
    {
        ServiceReply<bool> getReply = await TryGetData();

        return getReply.Success && _cachedResponse is not null
            ? new ServiceReply<List<CategoryResponse>?>( _cachedResponse.Object )
            : new ServiceReply<List<CategoryResponse>?>( getReply.ErrorType, getReply.Message );
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
    public async Task<ServiceReply<List<CategoryView>?>> GetCategoriesView()
    {
        try
        {
            IEnumerable<CategoryModel>? result = await _repository.Get();
            List<CategoryView>? dto = await MapView( result );

            return dto is not null
                ? new ServiceReply<List<CategoryView>?>( dto )
                : new ServiceReply<List<CategoryView>?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<List<CategoryView>?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<CategoryEdit?>> GetCategoryEdit( int categoryId )
    {
        try
        {
            CategoryModel? model = await _repository.GetEdit( categoryId );
            CategoryEdit? dto = MapEdit( model );

            return dto is not null
                ? new ServiceReply<CategoryEdit?>( dto )
                : new ServiceReply<CategoryEdit?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<CategoryEdit?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<bool>> AddBulkCategories( List<CategoryEdit> categories )
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
    public async Task<ServiceReply<int>> AddCategory( CategoryEdit dto )
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
    public async Task<ServiceReply<bool>> UpdateCategory( CategoryEdit dto )
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

        IEnumerable<CategoryModel>? models;

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
    
    async Task MapModels( IEnumerable<CategoryModel> models )
    {
        await Task.Run( () =>
        {
            List<CategoryModel> list = models.ToList();
            List<CategoryResponse> response = MapResponse( list );
            CategoryData data = new ( list );
            _cachedData = new CachedObject<CategoryData>( data );
            _cachedResponse = new CachedObject<List<CategoryResponse>>( response );
        } );
    }
    static List<CategoryResponse> MapResponse( IEnumerable<CategoryModel> models )
    {
        return models
            .Select( m => new CategoryResponse
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
    static async Task<List<CategoryView>?> MapView( IEnumerable<CategoryModel>? models )
    {
        if ( models is null )
            return null;

        return await Task.Run( () =>
        {
            return models
                .Select( m => new CategoryView { Id = m.CategoryId, Tier = m.Tier, Name = m.Name } )
                .ToList();
        } );
    }
    static CategoryEdit? MapEdit( CategoryModel? model )
    {
        if ( model is null )
            return null;

        return new CategoryEdit
        {
            CategoryId = model.CategoryId,
            ParentId = model.ParentCategoryId,
            Tier = model.Tier,
            Name = model.Name,
            ApiUrl = model.ApiUrl,
            ImageUrl = model.ImageUrl
        };
    }
}