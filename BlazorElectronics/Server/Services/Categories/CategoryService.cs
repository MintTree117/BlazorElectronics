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

        _cachedUrlMap = await MapResponseToUrlMap( categoriesResponse.Data );

        return new ApiReply<CategoryUrlMap?>( _cachedUrlMap );
    }
    async Task<ApiReply<CachedCategories?>> TryGetCategoriesDto()
    {
        if ( _cahcedCategories is not null )
            return new ApiReply<CachedCategories?>( _cahcedCategories );
        
        CategoriesModel? repositoryResponse;

        try
        {
            repositoryResponse = await _repository.GetCategories();

            if ( repositoryResponse is null )
                return new ApiReply<CachedCategories?>( NO_DATA_FOUND_MESSAGE );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ApiReply<CachedCategories?>( INTERNAL_SERVER_ERROR_MESSAGE);
        }

        _cahcedCategories = await MapCategoriesModelToDto( repositoryResponse );
        
        return new ApiReply<CachedCategories?>( _cahcedCategories );
    }
    
    static async Task<CachedCategories?> MapCategoriesModelToDto( CategoriesModel model )
    {
        return await Task.Run( () =>
        {
            if ( model.Primary is null || model.Secondary is null || model.Tertiary is null )
                return null;
            
            var primaryIds = new Dictionary<int, int>();
            var secondaryIds = new Dictionary<int, int>();
            var tertiaryIds = new Dictionary<int, int>();
            
            var primary = new List<PrimaryCategoryResponse>();
            var secondary = new List<SecondaryCategoryResponse>();
            var tertiary = new List<TertiaryCategoryResponse>();

            short count = 0;
            foreach ( PrimaryCategoryModel p in model.Primary )
            {
                if ( !primaryIds.TryAdd( p.PrimaryCategoryId, count ) )
                    continue;
                
                primary.Add( new PrimaryCategoryResponse
                {
                    Id = p.PrimaryCategoryId,
                    Name = p.Name,
                    Url = p.ApiUrl,
                    ImageUrl = p.ImageUrl,
                    ChildCategories = new List<int>(),
                });
                
                count++;
            }
            
            count = 0;
            foreach ( SecondaryCategoryModel s in model.Secondary )
            {
                if ( !primaryIds.ContainsKey( s.PrimaryCategoryId ) )
                    continue;
                if ( !secondaryIds.TryAdd( s.SecondaryCategoryId, count ) )
                    continue;
                 
                secondary.Add( new SecondaryCategoryResponse
                {
                    Id = s.SecondaryCategoryId,
                    ParentId = s.PrimaryCategoryId,
                    Name = s.Name,
                    Url = s.ApiUrl,
                    ImageUrl = s.ImageUrl,
                    ChildCategories = new List<int>()
                } );
                
                primary[ primaryIds[ s.PrimaryCategoryId ] ].ChildCategories.Add( s.SecondaryCategoryId );
                count++;
            }
            count = 0;
            
            foreach ( TertiaryCategoryModel t in model.Tertiary )
            {
                if ( !primaryIds.ContainsKey( t.PrimaryCategoryId ) )
                    continue;
                if ( !secondaryIds.ContainsKey( t.SecondaryCategoryId ) )
                    continue;
                if ( !tertiaryIds.TryAdd( t.TertiaryCategoryId, count ) )
                    continue;

                tertiary.Add( new TertiaryCategoryResponse
                {
                    Id = t.TertiaryCategoryId,
                    ParentId = t.SecondaryCategoryId,
                    Name = t.Name,
                    Url = t.ApiUrl,
                    ImageUrl = t.ImageUrl
                });
                
                secondary[ secondaryIds[ t.SecondaryCategoryId ] ].ChildCategories.Add( t.TertiaryCategoryId );
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
    static async Task<CategoryUrlMap> MapResponseToUrlMap( CachedCategories dto )
    {
        return await Task.Run( () =>
        {
            var primary = new Dictionary<string, int>();
            var secondary = new Dictionary<string, Dictionary<int, int>>();
            var tertiary = new Dictionary<string, Dictionary<int, int>>();
            
            foreach ( PrimaryCategoryResponse p in dto.PrimaryResponses )
            {
                primary.TryAdd( p.Url, p.Id );
            }

            foreach ( SecondaryCategoryResponse s in dto.SecondaryResponses )
            {
                if ( !secondary.TryGetValue( s.Url, out Dictionary<int, int>? secondaryMap ) )
                {
                    secondaryMap = new Dictionary<int, int>();
                    secondary.TryAdd( s.Url, secondaryMap );
                }
                
                secondaryMap.TryAdd( s.ParentId, s.Id );
            }

            foreach ( TertiaryCategoryResponse t in dto.TertiaryResponses )
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