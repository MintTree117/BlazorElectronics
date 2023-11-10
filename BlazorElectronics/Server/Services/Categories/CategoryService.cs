using BlazorElectronics.Server.Models.Categories;
using BlazorElectronics.Server.Repositories.Categories;
using BlazorElectronics.Shared.Mutual;
using BlazorElectronics.Shared.Outbound.Categories;

namespace BlazorElectronics.Server.Services.Categories;

public class CategoryService : ApiService, ICategoryService
{
    readonly ICategoryCache _cache;
    readonly ICategoryRepository _repository;

    const string INVALID_CATEGORY_MESSAGE = "Invalid Category!";

    CategoryUrlMap? _cachedUrlMap;
    CategoriesResponse? _cachedCategoriesResponse;
    IReadOnlyList<string>? _cachedMainDescriptions;

    public CategoryService( ILogger logger, ICategoryCache cache, ICategoryRepository repository ) : base( logger )
    {
        _cache = cache;
        _repository = repository;
    }
    
    public async Task<Reply<CategoriesResponse?>> GetCategories()
    {
        Reply<CategoriesResponse?> categoriesResponse = await TryGetCategoriesResponse();

        return categoriesResponse.Success
            ? categoriesResponse
            : new Reply<CategoriesResponse?>( categoriesResponse.Message );
    }
    public async Task<Reply<IReadOnlyList<string>?>> GetMainDescriptions()
    {
        if ( _cachedMainDescriptions is not null )
            return new Reply<IReadOnlyList<string>?>( _cachedMainDescriptions );
        
        IEnumerable<string>? descriptionReply = await _repository.GetPrimaryCategoryDescriptions();

        if ( descriptionReply is null )
            return new Reply<IReadOnlyList<string>?>( NO_DATA_FOUND_MESSAGE );

        _cachedMainDescriptions = descriptionReply.ToList();

        return new Reply<IReadOnlyList<string>?>( _cachedMainDescriptions );
    }
    public async Task<Reply<string?>> GetDescription( CategoryIdMap? idMap )
    {
        Reply<bool> validationReply = await ValidateCategoryIdMap( idMap );

        if ( idMap is null || !validationReply.Success )
            return new Reply<string?>( "Invalid category!" );

        try
        {
            string? descrReply = await _repository.GetCategoryDescription( idMap.CategoryId, idMap.CategoryTier );

            return string.IsNullOrWhiteSpace( descrReply )
                ? new Reply<string?>( NO_DATA_FOUND_MESSAGE )
                : new Reply<string?>( descrReply );
        }
        catch ( ServiceException e )
        {
            _logger.LogError( e, e.Message );
            return new Reply<string?>( INTERNAL_SERVER_ERROR_MESSAGE );
        }
    }
    public async Task<Reply<CategoryIdMap?>> GetCategoryIdMapFromUrl( string primaryUrl, string? secondaryUrl = null, string? tertiaryUrl = null )
    {
        Reply<CategoryUrlMap?> urlMapReply = await TryGetCategoryUrlMap();

        if ( !urlMapReply.Success || urlMapReply.Data is null )
            return new Reply<CategoryIdMap?>( urlMapReply.Message );

        List<string> urlList = GetCategoryUrlList( primaryUrl, secondaryUrl, tertiaryUrl );
        CategoryIdMap? idMap = urlMapReply.Data.GetCategoryIdMapFromUrl( urlList );

        return idMap is not null
            ? new Reply<CategoryIdMap?>( idMap )
            : new Reply<CategoryIdMap?>( INVALID_CATEGORY_MESSAGE );
    }
    public async Task<Reply<bool>> ValidateCategoryIdMap( CategoryIdMap? idMap )
    {
        if ( idMap is null )
            return new Reply<bool>( "CategoryIds are null!" );

        Reply<CategoriesResponse?> categoriesReply = await TryGetCategoriesResponse();
        
        if ( !categoriesReply.Success || categoriesReply.Data is null )
            return new Reply<bool>( categoriesReply.Message );

        return ValidateCategoryIdMap( idMap, categoriesReply.Data )
            ? new Reply<bool>( true )
            : new Reply<bool>( "Invalid Category!" );
    }

    async Task<Reply<CategoryUrlMap?>> TryGetCategoryUrlMap()
    {
        if ( _cachedUrlMap is not null )
            return new Reply<CategoryUrlMap?>( _cachedUrlMap );

        Reply<CategoriesResponse?> categoriesResponse = await TryGetCategoriesResponse();
        
        if ( !categoriesResponse.Success || categoriesResponse.Data is null )
            return new Reply<CategoryUrlMap?>( categoriesResponse.Message );

        _cachedUrlMap = await MapResponseToUrlMap( categoriesResponse.Data );

        return new Reply<CategoryUrlMap?>( _cachedUrlMap );
    }
    async Task<Reply<CategoriesResponse?>> TryGetCategoriesResponse()
    {
        if ( _cachedCategoriesResponse is not null )
            return new Reply<CategoriesResponse?>( _cachedCategoriesResponse );
        
        CategoriesModel? repositoryResponse;

        try
        {
            repositoryResponse = await _repository.GetCategories();

            if ( repositoryResponse is null )
                return new Reply<CategoriesResponse?>( NO_DATA_FOUND_MESSAGE );
        }
        catch ( ServiceException e )
        {
            _logger.LogError( e.Message, e );
            return new Reply<CategoriesResponse?>( INTERNAL_SERVER_ERROR_MESSAGE);
        }

        _cachedCategoriesResponse = await MapCategoriesModelToResponse( repositoryResponse );
        
        return new Reply<CategoriesResponse?>( _cachedCategoriesResponse );
    }

    static async Task<CategoriesResponse> MapCategoriesModelToResponse( CategoriesModel model )
    {
        return await Task.Run( () =>
        {
            var primaryIds = new HashSet<short>();
            var secondaryIds = new HashSet<short>();
            var tertiaryIds = new HashSet<short>();

            var primaryNames = new Dictionary<short, string>();
            var secondaryNames = new Dictionary<short, string>();
            var tertiaryNames = new Dictionary<short, string>();

            var primaryUrls = new Dictionary<short, string>();
            var secondaryUrls = new Dictionary<short, string>();
            var tertiaryUrls = new Dictionary<short, string>();

            var primaryImages = new Dictionary<short, string>();
            var secondaryImages = new Dictionary<short, string>();
            var tertiaryImages = new Dictionary<short, string>();

            var secondaryParents = new Dictionary<short, short>();
            var tertiaryParents = new Dictionary<short, short>();

            var primaryChildren = new Dictionary<short, HashSet<short>>();
            var secondaryChildren = new Dictionary<short, HashSet<short>>();

            foreach ( PrimaryCategory p in model.Primary! )
            {
                primaryIds.Add( p.PrimaryCategoryId );
                primaryNames.Add( p.PrimaryCategoryId, p.Name );
                primaryUrls.Add( p.PrimaryCategoryId, p.ApiUrl );
                primaryImages.Add( p.PrimaryCategoryId, p.ImageUrl );

                if ( primaryChildren.ContainsKey( p.PrimaryCategoryId ) )
                    continue;

                primaryChildren.Add( p.PrimaryCategoryId, new HashSet<short>() );
            }

            foreach ( SecondaryCategory s in model.Secondary! )
            {
                if ( primaryIds.Contains( s.PrimaryCategoryId ) )
                    continue;
                if ( !primaryChildren.TryGetValue( s.PrimaryCategoryId, out HashSet<short>? primaryChildrenSet ) )
                    continue;
                if ( secondaryChildren.ContainsKey( s.SecondaryCategoryId ) )
                    continue;
                
                secondaryParents.Add( s.SecondaryCategoryId, s.PrimaryCategoryId );
                secondaryIds.Add( s.SecondaryCategoryId );
                secondaryNames.Add( s.SecondaryCategoryId, s.Name );
                secondaryUrls.Add( s.SecondaryCategoryId, s.ApiUrl );
                secondaryImages.Add( s.SecondaryCategoryId, s.ImageUrl );

                secondaryChildren.Add( s.SecondaryCategoryId, new HashSet<short>() );
                primaryChildrenSet.Add( s.SecondaryCategoryId );
            }

            foreach ( TertiaryCategory t in model.Tertiary! )
            {
                if ( !primaryIds.Contains( t.PrimaryCategoryId ) )
                    continue;
                if ( !secondaryIds.Contains( t.SecondaryCategoryId ) )
                    continue;

                tertiaryParents.Add( t.TertiaryCategoryId, t.SecondaryCategoryId );
                tertiaryIds.Add( t.TertiaryCategoryId );
                tertiaryNames.Add( t.TertiaryCategoryId, t.Name );
                tertiaryUrls.Add( t.TertiaryCategoryId, t.ApiUrl );
                tertiaryImages.Add( t.TertiaryCategoryId, t.ImageUrl );

                if ( !secondaryChildren.TryGetValue( t.SecondaryCategoryId, out HashSet<short>? secondaryChildrenSet ) )
                    continue;
                
                secondaryChildrenSet.Add( t.SecondaryCategoryId );
            }

            var primaryResponses = new Dictionary<short, PrimaryCategoryResponse>();
            var secondaryResponses = new Dictionary<short, SecondaryCategoryResponse>();
            var tertiaryResponses = new Dictionary<short, TertiaryCategoryResponse>();

            foreach ( short id in primaryIds )
            {
                var primaryResponse = new PrimaryCategoryResponse(
                    id, primaryNames[ id ], primaryUrls[ id ], primaryImages[ id ], primaryChildren[ id ] );

                primaryResponses.Add( id, primaryResponse );
            }
            foreach ( short id in secondaryIds )
            {
                var secondaryResponse = new SecondaryCategoryResponse(
                    secondaryParents[ id ], id, secondaryNames[ id ], secondaryUrls[ id ], secondaryImages[ id ], secondaryChildren[ id ] );

                secondaryResponses.Add( id, secondaryResponse );
            }
            foreach ( short id in tertiaryIds )
            {
                var tertiaryResponse = new TertiaryCategoryResponse(
                    tertiaryParents[ id ], id, tertiaryNames[ id ], tertiaryUrls[ id ], tertiaryImages[ id ] );

                tertiaryResponses.Add( id, tertiaryResponse );
            }

            return new CategoriesResponse( primaryResponses, secondaryResponses, tertiaryResponses );
        } );
    }
    static async Task<CategoryUrlMap> MapResponseToUrlMap( CategoriesResponse response )
    {
        return await Task.Run( () =>
        {
            var primary = new Dictionary<string, short>();
            var secondary = new Dictionary<string, Dictionary<short, short>>();
            var tertiary = new Dictionary<string, Dictionary<short, short>>();

            var secondaryNames = new List<string>();
            var tertiaryNames = new List<string>();
            
            foreach ( PrimaryCategoryResponse p in response.PrimaryCategories.Values )
            {
                if ( primary.ContainsValue( p.Id ) )
                    continue;

                primary.Add( p.Url, p.Id );
            }

            foreach ( SecondaryCategoryResponse s in response.SecondaryCategories.Values )
            {
                if ( !response.PrimaryCategories.ContainsKey( s.ParentId ) )
                    continue;

                secondaryNames.Add( s.Name );

                if ( !secondary.TryGetValue( s.Url, out Dictionary<short, short>? secondaryMap ) )
                {
                    secondaryMap = new Dictionary<short, short>();
                    secondary.Add( s.Url, secondaryMap );
                }

                secondaryMap.TryAdd( s.ParentId, s.Id );
            }

            foreach ( TertiaryCategoryResponse t in response.TertiaryCategories.Values )
            {
                if ( !response.SecondaryCategories.TryGetValue( t.ParentId, out SecondaryCategoryResponse? secondaryCategory ) )
                    continue;

                if ( !secondary.TryGetValue( secondaryCategory.Url, out Dictionary<short, short>? secondaryMap ) )
                    continue;

                tertiaryNames.Add( t.Name );

                if ( !tertiary.TryGetValue( t.Url, out Dictionary<short, short>? tertiaryMap ) )
                {
                    tertiaryMap = new Dictionary<short, short>();
                    tertiary.Add( t.Url, tertiaryMap );
                }

                tertiaryMap.TryAdd( t.ParentId, t.Id );
            }

            var readonlySecondaries = new List<IReadOnlyDictionary<short, short>>();
            var readonlyTertiaries = new List<IReadOnlyDictionary<short, short>>();

            foreach ( Dictionary<short, short> s in secondary.Values )
                readonlySecondaries.Add( new Dictionary<short, short>( s ) );
            foreach ( Dictionary<short, short> t in tertiary.Values )
                readonlyTertiaries.Add( new Dictionary<short, short>( t ) );

            var sDict = new Dictionary<string, IReadOnlyDictionary<short, short>>();
            var tDict = new Dictionary<string, IReadOnlyDictionary<short, short>>();

            for ( int i = 0; i < secondaryNames.Count; i++ )
                sDict.Add( secondaryNames[ i ], readonlySecondaries[ i ] );
            for ( int i = 0; i < tertiaryNames.Count; i++ )
                tDict.Add( tertiaryNames[ i ], readonlyTertiaries[ i ] );

            return new CategoryUrlMap( primary, sDict, tDict );
        } );
    }
    static bool ValidateCategoryIdMap( CategoryIdMap map, CategoriesResponse categories )
    {
        return map.CategoryTier switch {
            1 => categories.PrimaryCategories.ContainsKey( map.CategoryId ),
            2 => categories.SecondaryCategories.ContainsKey( map.CategoryId ),
            3 => categories.TertiaryCategories.ContainsKey( map.CategoryId ),
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