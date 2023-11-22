using BlazorElectronics.Server.Dtos.Categories;
using BlazorElectronics.Server.Models.Categories;
using BlazorElectronics.Server.Repositories.Categories;
using BlazorElectronics.Shared.Mutual;
using BlazorElectronics.Shared.Outbound.Categories;

namespace BlazorElectronics.Server.Services.Categories;

public class CategoryService : ApiService, ICategoryService
{
    readonly ICategoryRepository _repository;

    const string INVALID_CATEGORY_MESSAGE = "Invalid Category!";

    CategoryUrlMap? _cachedUrlMap;
    CategoriesDto? _cachedCategoriesDto;
    IReadOnlyList<string>? _cachedMainDescriptions;

    public CategoryService( ILogger<ApiService> logger, ICategoryRepository repository ) : base( logger )
    {
        _repository = repository;
    }

    public async Task<ApiReply<CategoriesDto?>> GetCategoriesDto()
    {
        ApiReply<CategoriesDto?> categoriesReply = await TryGetCategoriesDto();

        return categoriesReply.Success
            ? new ApiReply<CategoriesDto?>( _cachedCategoriesDto )
            : new ApiReply<CategoriesDto?>( categoriesReply.Message );
    }
    public async Task<ApiReply<CategoriesResponse?>> GetCategories()
    {
        ApiReply<CategoriesDto?> categoriesReply = await TryGetCategoriesDto();

        if ( !categoriesReply.Success || categoriesReply.Data is null )
            return new ApiReply<CategoriesResponse?>( NO_DATA_FOUND_MESSAGE );

        CategoriesResponse? response = GetCategoriesResponse( categoriesReply.Data );

        return response is not null
            ? new ApiReply<CategoriesResponse?>( response )
            : new ApiReply<CategoriesResponse?>( NO_DATA_FOUND_MESSAGE );
    }
    public async Task<ApiReply<IReadOnlyList<string>?>> GetMainDescriptions()
    {
        if ( _cachedMainDescriptions is not null )
            return new ApiReply<IReadOnlyList<string>?>( _cachedMainDescriptions );
        
        IEnumerable<string>? descriptionReply = await _repository.GetPrimaryCategoryDescriptions();

        if ( descriptionReply is null )
            return new ApiReply<IReadOnlyList<string>?>( NO_DATA_FOUND_MESSAGE );

        _cachedMainDescriptions = descriptionReply.ToList();

        return new ApiReply<IReadOnlyList<string>?>( _cachedMainDescriptions );
    }
    public async Task<ApiReply<string?>> GetDescription( CategoryIdMap? idMap )
    {
        ApiReply<bool> validationReply = await ValidateCategoryIdMap( idMap );

        if ( idMap is null || !validationReply.Success )
            return new ApiReply<string?>( "Invalid category!" );

        try
        {
            string? descrReply = await _repository.GetCategoryDescription( idMap.CategoryType, idMap.CategoryId );

            return string.IsNullOrWhiteSpace( descrReply )
                ? new ApiReply<string?>( NO_DATA_FOUND_MESSAGE )
                : new ApiReply<string?>( descrReply );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e, e.Message );
            return new ApiReply<string?>( INTERNAL_SERVER_ERROR_MESSAGE );
        }
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

        ApiReply<CategoriesDto?> categoriesReply = await TryGetCategoriesDto();
        
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

        ApiReply<CategoriesDto?> categoriesResponse = await TryGetCategoriesDto();
        
        if ( !categoriesResponse.Success || categoriesResponse.Data is null )
            return new ApiReply<CategoryUrlMap?>( categoriesResponse.Message );

        _cachedUrlMap = await MapResponseToUrlMap( categoriesResponse.Data );

        return new ApiReply<CategoryUrlMap?>( _cachedUrlMap );
    }
    async Task<ApiReply<CategoriesDto?>> TryGetCategoriesDto()
    {
        if ( _cachedCategoriesDto is not null )
            return new ApiReply<CategoriesDto?>( _cachedCategoriesDto );
        
        CategoriesModel? repositoryResponse;

        try
        {
            repositoryResponse = await _repository.GetCategories();

            if ( repositoryResponse is null )
                return new ApiReply<CategoriesDto?>( NO_DATA_FOUND_MESSAGE );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ApiReply<CategoriesDto?>( INTERNAL_SERVER_ERROR_MESSAGE);
        }

        _cachedCategoriesDto = await MapCategoriesModelToDto( repositoryResponse );
        
        return new ApiReply<CategoriesDto?>( _cachedCategoriesDto );
    }
    
    static async Task<CategoriesDto?> MapCategoriesModelToDto( CategoriesModel model )
    {
        return await Task.Run( () =>
        {
            if ( model.Primary is null || model.Secondary is null || model.Tertiary is null )
                return null;
            
            var primaryIds = new Dictionary<short, short>();
            var secondaryIds = new Dictionary<short, short>();
            var tertiaryIds = new Dictionary<short, short>();
            
            var tempPrimaryResponses = new List<PrimaryCategoryResponse>();
            var tempSecondaryResponses = new List<SecondaryCategoryResponse>();
            var tempTertiaryResponses = new List<TertiaryCategoryResponse>();

            var primaryChildren = new Dictionary<short, HashSet<short>>();
            var secondaryChildren = new Dictionary<short, HashSet<short>>();

            short count = 0;
            foreach ( PrimaryCategoryModel p in model.Primary )
            {
                if ( !primaryIds.TryAdd( p.PrimaryCategoryId, count ) )
                    continue;
                
                tempPrimaryResponses.Add( new PrimaryCategoryResponse
                {
                    Id = p.PrimaryCategoryId,
                    Name = p.Name,
                    Url = p.ApiUrl,
                    ImageUrl = p.ImageUrl,
                    ChildCategories = new HashSet<short>()
                });
                primaryChildren.TryAdd( p.PrimaryCategoryId, new HashSet<short>() );
                count++;
            }
            count = 0;
            foreach ( SecondaryCategoryModel s in model.Secondary )
            {
                if ( !primaryIds.ContainsKey( s.PrimaryCategoryId ) )
                    continue;
                if ( !secondaryIds.TryAdd( s.SecondaryCategoryId, count ) )
                    continue;
                 
                tempSecondaryResponses.Add( new SecondaryCategoryResponse
                {
                    Id = s.SecondaryCategoryId,
                    ParentId = s.PrimaryCategoryId,
                    Name = s.Name,
                    Url = s.ApiUrl,
                    ImageUrl = s.ImageUrl,
                    ChildCategories = new HashSet<short>()
                } );
                secondaryChildren.TryAdd( s.SecondaryCategoryId, new HashSet<short>() );
                primaryChildren[ s.PrimaryCategoryId ].Add( s.SecondaryCategoryId );
                
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

                tempTertiaryResponses.Add( new TertiaryCategoryResponse
                {
                    Id = t.TertiaryCategoryId,
                    ParentId = t.SecondaryCategoryId,
                    Name = t.Name,
                    Url = t.ApiUrl,
                    ImageUrl = t.ImageUrl
                });
                secondaryChildren[ t.SecondaryCategoryId ].Add( t.TertiaryCategoryId );
                count++;
            }

            var primaryResponses = new List<PrimaryCategoryResponse>();
            var secondaryResponses = new List<SecondaryCategoryResponse>();
            var tertiaryResponses = new List<TertiaryCategoryResponse>();

            foreach ( short id in primaryIds.Keys )
            {
                PrimaryCategoryResponse p = tempPrimaryResponses[ primaryIds[ id ] ];
                HashSet<short> c = primaryChildren[ id ];

                primaryResponses.Add(
                    new PrimaryCategoryResponse
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Url = p.Url,
                        ImageUrl = p.ImageUrl,
                        ChildCategories = c
                    } );
            }
            foreach ( short id in secondaryIds.Keys )
            {
                SecondaryCategoryResponse s = tempSecondaryResponses[ secondaryIds[ id ] ];
                HashSet<short> c = secondaryChildren[ id ];

                secondaryResponses.Add( 
                    new SecondaryCategoryResponse
                    {
                        Id = s.Id,
                        ParentId = s.ParentId,
                        Name = s.Name,
                        Url = s.Url,
                        ImageUrl = s.ImageUrl,
                        ChildCategories = c
                    } );
            }
            foreach ( short id in tertiaryIds.Keys )
            {
                TertiaryCategoryResponse t = tempTertiaryResponses[ tertiaryIds[ id ] ];

                tertiaryResponses.Add(
                    new TertiaryCategoryResponse
                    {
                        Id = t.Id,
                        ParentId = t.ParentId,
                        Name = t.Name,
                        Url = t.Url,
                        ImageUrl = t.ImageUrl
                    } );
            }

            return new CategoriesDto( primaryIds, secondaryIds, tertiaryIds, primaryResponses, secondaryResponses, tertiaryResponses );
        } );
    }
    static async Task<CategoryUrlMap> MapResponseToUrlMap( CategoriesDto dto )
    {
        return await Task.Run( () =>
        {
            var primary = new Dictionary<string, short>();
            var secondary = new Dictionary<string, Dictionary<short, short>>();
            var tertiary = new Dictionary<string, Dictionary<short, short>>();
            
            foreach ( short categoryId in dto.PrimaryIds.Keys )
            {
                short responseId = dto.PrimaryIds[ categoryId ];
                PrimaryCategoryResponse response = dto.PrimaryResponses[ responseId ];
                primary.TryAdd( response.Url, response.Id );
            }

            foreach ( short categoryId in dto.SecondaryIds.Keys )
            {
                short responseId = dto.SecondaryIds[ categoryId ];
                SecondaryCategoryResponse response = dto.SecondaryResponses[ responseId ];
                
                if ( !secondary.TryGetValue( response.Url, out Dictionary<short, short>? secondaryMap ) )
                {
                    secondaryMap = new Dictionary<short, short>();
                    secondary.TryAdd( response.Url, secondaryMap );
                }
                
                secondaryMap.TryAdd( response.ParentId, response.Id );
            }

            foreach ( short categoryId in dto.TertiaryIds.Keys )
            {
                short responseId = dto.TertiaryIds[ categoryId ];
                TertiaryCategoryResponse response = dto.TertiaryResponses[ responseId ];
                
                if ( !tertiary.TryGetValue( response.Url, out Dictionary<short, short>? tertiaryMap ) )
                {
                    tertiaryMap = new Dictionary<short, short>();
                    tertiary.Add( response.Url, tertiaryMap );
                }

                tertiaryMap.TryAdd( response.ParentId, response.Id );
            }
            
            return new CategoryUrlMap( primary, secondary, tertiary );
        } );
    }
    static CategoriesResponse? GetCategoriesResponse( CategoriesDto dto )
    {
        return new CategoriesResponse( dto.PrimaryResponses.ToList(), dto.SecondaryResponses.ToList(), dto.TertiaryResponses.ToList() );
    }
    static bool ValidateCategoryIdMap( CategoryIdMap map, CategoriesDto categories )
    {
        return map.CategoryType switch {
            CategoryType.PRIMARY => categories.PrimaryIds.ContainsKey( map.CategoryId ),
            CategoryType.SECONDARY => categories.SecondaryIds.ContainsKey( map.CategoryId ),
            CategoryType.TERTIARY => categories.TertiaryIds.ContainsKey( map.CategoryId ),
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