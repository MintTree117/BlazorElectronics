using BlazorElectronics.Server.Dtos.Categories;
using BlazorElectronics.Server.Models.Categories;
using BlazorElectronics.Server.Repositories.Categories;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Server.Services.Categories;

public class CategoryService : ApiService, ICategoryService
{
    readonly ICategoryRepository _repository;

    const string INVALID_CATEGORY_MESSAGE = "Invalid Category!";

    CategoryUrlMap? _cachedUrlMap;
    CachedCategories? _cahcedCategories;

    public CategoryService( ILogger<ApiService> logger, ICategoryRepository repository ) : base( logger )
    {
        _repository = repository;
    }

    public async Task<ApiReply<CachedCategories?>> GetCategoriesDto()
    {
        ApiReply<CachedCategories?> categoriesReply = await TryGetCategoriesDto();

        return categoriesReply.Success
            ? new ApiReply<CachedCategories?>( _cahcedCategories )
            : new ApiReply<CachedCategories?>( categoriesReply.Message );
    }
    public async Task<ApiReply<CategoriesResponse?>> GetCategoriesResponse()
    {
        ApiReply<CachedCategories?> categoriesReply = await TryGetCategoriesDto();

        if ( !categoriesReply.Success || _cahcedCategories is null )
            return new ApiReply<CategoriesResponse?>( categoriesReply.Message );

        var response = new CategoriesResponse( _cahcedCategories.PrimaryResponses, _cahcedCategories.SecondaryResponses, _cahcedCategories.TertiaryResponses );
        return new ApiReply<CategoriesResponse?>( response );
    }
    public async Task<ApiReply<CategoryIdMap?>> GetCategoryIdMapFromUrl( string primaryUrl, string? secondaryUrl = null, string? tertiaryUrl = null )
    {
        ApiReply<CategoryUrlMap?> urlMapReply = await TryGetCategoryUrlMap();

        if ( !urlMapReply.Success || urlMapReply.Data is null )
            return new ApiReply<CategoryIdMap?>( urlMapReply.Message );

        List<string> urlList = GetCategoryUrlList( primaryUrl, secondaryUrl, tertiaryUrl );
        CategoryIdMap? idMap = urlMapReply.Data.GetCategoryIdMapFromUrl( urlList );

        return idMap is not null
            ? new ApiReply<CategoryIdMap?>( idMap )
            : new ApiReply<CategoryIdMap?>( INVALID_CATEGORY_MESSAGE );
    }
    public async Task<ApiReply<bool>> ValidateCategoryIdMap( CategoryIdMap? idMap )
    {
        if ( idMap is null )
            return new ApiReply<bool>( "CategoryIds are null!" );

        ApiReply<CachedCategories?> categoriesReply = await TryGetCategoriesDto();
        
        if ( !categoriesReply.Success || categoriesReply.Data is null )
            return new ApiReply<bool>( categoriesReply.Message );

        return ValidateCategoryIdMap( idMap, categoriesReply.Data )
            ? new ApiReply<bool>( true )
            : new ApiReply<bool>( "Invalid Category!" );
    }

    async Task<ApiReply<CategoryUrlMap?>> TryGetCategoryUrlMap()
    {
        if ( _cachedUrlMap is not null )
            return new ApiReply<CategoryUrlMap?>( _cachedUrlMap );

        ApiReply<CachedCategories?> categoriesResponse = await TryGetCategoriesDto();
        
        if ( !categoriesResponse.Success || categoriesResponse.Data is null )
            return new ApiReply<CategoryUrlMap?>( categoriesResponse.Message );
        
        _cachedUrlMap = await CreateUrlMap( categoriesResponse.Data );
        return new ApiReply<CategoryUrlMap?>( _cachedUrlMap );
    }
    async Task<ApiReply<CachedCategories?>> TryGetCategoriesDto()
    {
        if ( _cahcedCategories is not null )
            return new ApiReply<CachedCategories?>( _cahcedCategories );
        
        CategoriesModel? repositoryResponse;

        try
        {
            repositoryResponse = await _repository.Get();

            if ( repositoryResponse is null )
                return new ApiReply<CachedCategories?>( NO_DATA_FOUND_MESSAGE );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ApiReply<CachedCategories?>( INTERNAL_SERVER_ERROR_MESSAGE);
        }

        _cahcedCategories = await CreateCacheFromModel( repositoryResponse );
        
        return new ApiReply<CachedCategories?>( _cahcedCategories );
    }
    
    static async Task<CachedCategories?> CreateCacheFromModel( CategoriesModel model )
    {
        return await Task.Run( () =>
        {
            if ( model.Primary is null || model.Secondary is null || model.Tertiary is null )
                return null;
            
            var primaryIds = new Dictionary<int, int>();
            var secondaryIds = new Dictionary<int, int>();
            var tertiaryIds = new Dictionary<int, int>();
            
            var primary = new List<CategoryResponse>();
            var secondary = new List<CategoryResponse>();
            var tertiary = new List<CategoryResponse>();

            short count = 0;
            foreach ( CategoryModel p in model.Primary )
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
            foreach ( CategoryModel s in model.Secondary )
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
            
            foreach ( CategoryModel t in model.Tertiary )
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

            return new CachedCategories
            {
                PrimarySet = new HashSet<int>( primaryIds.Keys ),
                SecondarySet = new HashSet<int>( secondaryIds.Keys ),
                TertiarySet = new HashSet<int>( tertiaryIds.Keys ),
                PrimaryResponses = primary,
                SecondaryResponses = secondary,
                TertiaryResponses = tertiary
            };
        } );
    }
    static async Task<CategoryUrlMap> CreateUrlMap( CachedCategories dto )
    {
        return await Task.Run( () =>
        {
            var primary = new Dictionary<string, int>();
            var secondary = new Dictionary<string, Dictionary<int, int>>();
            var tertiary = new Dictionary<string, Dictionary<int, int>>();
            
            foreach ( CategoryResponse p in dto.PrimaryResponses )
            {
                primary.TryAdd( p.Url, p.Id );
            }

            foreach ( CategoryResponse s in dto.SecondaryResponses )
            {
                if ( !secondary.TryGetValue( s.Url, out Dictionary<int, int>? secondaryMap ) )
                {
                    secondaryMap = new Dictionary<int, int>();
                    secondary.TryAdd( s.Url, secondaryMap );
                }
                
                secondaryMap.TryAdd( s.ParentId, s.Id );
            }

            foreach ( CategoryResponse t in dto.TertiaryResponses )
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
    
    static bool ValidateCategoryIdMap( CategoryIdMap map, CachedCategories cachedCategories )
    {
        return map.CategoryType switch {
            CategoryType.PRIMARY => cachedCategories.PrimarySet.Contains( map.CategoryId ),
            CategoryType.SECONDARY => cachedCategories.SecondarySet.Contains( map.CategoryId ),
            CategoryType.TERTIARY => cachedCategories.TertiarySet.Contains( map.CategoryId ),
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