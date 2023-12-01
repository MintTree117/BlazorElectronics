using BlazorElectronics.Server.Dtos;
using BlazorElectronics.Server.Dtos.Categories;
using BlazorElectronics.Server.Models.Categories;
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

    public CategoryService( ILogger<ApiService> logger, ICategoryRepository repository ) : base( logger )
    {
        _repository = repository;
    }
    
    public async Task<ServiceReply<CategoriesResponse?>> GetCategoriesResponse()
    {
        ServiceReply<bool> getReply = await TryGetData();

        return getReply.Success && _cachedData is not null
            ? new ServiceReply<CategoriesResponse?>( _cachedData.Object.Response )
            : new ServiceReply<CategoriesResponse?>( getReply.ErrorType, getReply.Message );
    }
    public async Task<ServiceReply<CategoryIdMap?>> GetCategoryIdMapFromUrl( string primaryUrl, string? secondaryUrl = null, string? tertiaryUrl = null )
    {
        ServiceReply<bool> getReply = await TryGetData();

        if ( !getReply.Success || _cachedData is null )
            return new ServiceReply<CategoryIdMap?>( getReply.ErrorType, getReply.Message );

        List<string> urlList = GetCategoryUrlList( primaryUrl, secondaryUrl, tertiaryUrl );
        CategoryIdMap? idMap = _cachedData.Object.Urls.GetCategoryIdMapFromUrl( urlList );

        return idMap is not null
            ? new ServiceReply<CategoryIdMap?>( idMap )
            : new ServiceReply<CategoryIdMap?>( ServiceErrorType.ValidationError, INVALID_CATEGORY_MESSAGE );
    }
    public async Task<ServiceReply<bool>> ValidateCategoryIdMap( CategoryIdMap idMap )
    {
        ServiceReply<bool> getReply = await TryGetData();
        
        if ( !getReply.Success || _cachedData is null )
            return new ServiceReply<bool>( getReply.ErrorType, getReply.Message );

        return ValidateCategoryIdMap( idMap, _cachedData.Object.Ids )
            ? new ServiceReply<bool>( true )
            : new ServiceReply<bool>( ServiceErrorType.ValidationError, INVALID_CATEGORY_MESSAGE );
    }
    public async Task<ServiceReply<CategoriesViewDto?>> GetCategoriesView()
    {
        try
        {
            CategoriesViewDto? result = await _repository.GetView();

            return result is not null
                ? new ServiceReply<CategoriesViewDto?>( result )
                : new ServiceReply<CategoriesViewDto?>( ServiceErrorType.NotFound );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<CategoriesViewDto?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<CategoryEditDto?>> GetCategoryEdit( CategoryGetEditDto dto )
    {
        try
        {
            CategoryEditDto? result = await _repository.GetEdit( dto );

            return result is not null
                ? new ServiceReply<CategoryEditDto?>( result )
                : new ServiceReply<CategoryEditDto?>( ServiceErrorType.NotFound );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<CategoryEditDto?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<CategoryEditDto?>> AddCategory( CategoryAddDto dto )
    {
        try
        {
            CategoryEditDto? result = await _repository.Insert( dto );

            return result is not null
                ? new ServiceReply<CategoryEditDto?>( result )
                : new ServiceReply<CategoryEditDto?>( ServiceErrorType.NotFound );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<CategoryEditDto?>( ServiceErrorType.ServerError );
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
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<bool>> RemoveCategory( CategoryRemoveDto dto )
    {
        try
        {
            bool result = await _repository.Delete( dto );

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

    async Task<ServiceReply<bool>> TryGetData()
    {
        if ( _cachedData is not null && _cachedData.IsValid( CACHE_LIFE ) )
            return new ServiceReply<bool>( true );

        CategoriesModel? repositoryResponse;

        try
        {
            repositoryResponse = await _repository.Get();
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }

        if ( repositoryResponse is null )
            return new ServiceReply<bool>( ServiceErrorType.NotFound );
        
        if ( repositoryResponse.Primary is null || repositoryResponse.Secondary is null || repositoryResponse.Tertiary is null )
            return new ServiceReply<bool>( ServiceErrorType.NotFound );

        await CreateCacheFromModel( repositoryResponse );
        return new ServiceReply<bool>( true );
    }
    
    async Task CreateCacheFromModel( CategoriesModel model )
    {
        CategoryIds ids = null;
        CategoriesResponse response = null;
        CategoryUrlMap map;
        
        await Task.Run( () =>
        {
            var primaryIds = new Dictionary<int, int>();
            var secondaryIds = new Dictionary<int, int>();
            var tertiaryIds = new Dictionary<int, int>();
            
            var primary = new List<CategoryResponse>();
            var secondary = new List<CategoryResponse>();
            var tertiary = new List<CategoryResponse>();

            short count = 0;
            foreach ( CategoryModel p in model.Primary! )
            {
                if ( !primaryIds.TryAdd( p.PrimaryCategoryId, count ) )
                    continue;
                
                primary.Add( new CategoryResponse
                {
                    Id = p.PrimaryCategoryId,
                    Name = p.Name,
                    Url = p.ApiUrl,
                    ImageUrl = p.ImageUrl,
                    Children = new List<int>(),
                });
                
                count++;
            }
            
            count = 0;
            foreach ( CategoryModel s in model.Secondary! )
            {
                bool valid =
                    primaryIds.ContainsKey( s.PrimaryCategoryId ) &&
                    secondaryIds.TryAdd( s.SecondaryCategoryId, count );

                if ( !valid )
                    continue;

                secondary.Add( new CategoryResponse
                {
                    Id = s.SecondaryCategoryId,
                    ParentId = s.PrimaryCategoryId,
                    Name = s.Name,
                    Url = s.ApiUrl,
                    ImageUrl = s.ImageUrl,
                    Children = new List<int>()
                } );
                
                primary[ primaryIds[ s.PrimaryCategoryId ] ].Children.Add( s.SecondaryCategoryId );
                count++;
            }
            count = 0;
            
            foreach ( CategoryModel t in model.Tertiary! )
            {
                bool valid = 
                    primaryIds.ContainsKey( t.PrimaryCategoryId ) &&
                    secondaryIds.ContainsKey( t.SecondaryCategoryId ) &&
                    tertiaryIds.TryAdd( t.TertiaryCategoryId, count );

                if ( !valid )
                    continue;

                tertiary.Add( new CategoryResponse
                {
                    Id = t.TertiaryCategoryId,
                    ParentId = t.SecondaryCategoryId,
                    Name = t.Name,
                    Url = t.ApiUrl,
                    ImageUrl = t.ImageUrl
                });
                
                secondary[ secondaryIds[ t.SecondaryCategoryId ] ].Children.Add( t.TertiaryCategoryId );
                count++;
            }

            ids = new CategoryIds
            {
                PrimarySet = new HashSet<int>( primaryIds.Keys ),
                SecondarySet = new HashSet<int>( secondaryIds.Keys ),
                TertiarySet = new HashSet<int>( tertiaryIds.Keys ),
            };

            response = new CategoriesResponse
            {

                Primary = primary,
                Secondary = secondary,
                Tertiary = tertiary
            };
        } );

        map = await CreateUrlMap( response! );

        var data = new CategoryData
        {
            Ids = ids!,
            Response = response!,
            Urls = map
        };

        _cachedData = new CachedObject<CategoryData>( data );
    }
    async Task<CategoryUrlMap> CreateUrlMap( CategoriesResponse dto )
    {
        return await Task.Run( () =>
        {
            var primary = new Dictionary<string, int>();
            var secondary = new Dictionary<string, Dictionary<int, int>>();
            var tertiary = new Dictionary<string, Dictionary<int, int>>();
            
            foreach ( CategoryResponse p in dto.Primary )
            {
                primary.TryAdd( p.Url, p.Id );
            }

            foreach ( CategoryResponse s in dto.Secondary )
            {
                if ( !secondary.TryGetValue( s.Url, out Dictionary<int, int>? secondaryMap ) )
                {
                    secondaryMap = new Dictionary<int, int>();
                    secondary.TryAdd( s.Url, secondaryMap );
                }
                
                secondaryMap.TryAdd( s.ParentId, s.Id );
            }

            foreach ( CategoryResponse t in dto.Tertiary )
            {
                if ( !tertiary.TryGetValue( t.Url, out Dictionary<int, int>? tertiaryMap ) )
                {
                    tertiaryMap = new Dictionary<int, int>();
                    tertiary.Add( t.Url, tertiaryMap );
                }

                tertiaryMap.TryAdd( t.ParentId, t.Id );
            }

            return new CategoryUrlMap( primary, secondary, tertiary );
        } );
    }
    
    static bool ValidateCategoryIdMap( CategoryIdMap map, CategoryIds ids )
    {
        return map.CategoryType switch {
            CategoryType.PRIMARY => ids.PrimarySet.Contains( map.CategoryId ),
            CategoryType.SECONDARY => ids.SecondarySet.Contains( map.CategoryId ),
            CategoryType.TERTIARY => ids.TertiarySet.Contains( map.CategoryId ),
            _ => false
        };
    }
    static List<string> GetCategoryUrlList( string primary, string? secondary = null, string? tertiary = null )
    {
        var urlCategories = new List<string> { primary };

        if ( !string.IsNullOrEmpty( secondary ) )
            urlCategories.Add( secondary );

        if ( !string.IsNullOrEmpty( tertiary ) )
            urlCategories.Add( tertiary );

        return urlCategories;
    }
}