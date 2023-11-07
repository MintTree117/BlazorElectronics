using BlazorElectronics.Server.Models.Categories;
using BlazorElectronics.Server.Repositories.Categories;
using BlazorElectronics.Shared.Mutual;
using BlazorElectronics.Shared.Outbound.Categories;

namespace BlazorElectronics.Server.Services.Categories;

public class CategoryService : ICategoryService
{
    readonly ICategoryCache _cache;
    readonly ICategoryRepository _repository;

    public CategoryService( ICategoryCache cache, ICategoryRepository repository )
    {
        _cache = cache;
        _repository = repository;
    }
    
    public async Task<Reply<CategoriesResponse?>> GetCategories()
    {
        Reply<CategoriesResponse?> categoriesResponse = await GetCategoriesResponse();

        if ( !categoriesResponse.Success )
            return new Reply<CategoriesResponse?>( categoriesResponse.Message );

        Reply<bool> cacheResponse = await CacheCategoriesResponse( categoriesResponse.Data! );

        return new Reply<CategoriesResponse?>( 
            categoriesResponse.Data, true, $"Fetch results: {categoriesResponse.Message} Cache results: {cacheResponse.Message}" );
    }
    public async Task<Reply<List<string?>?>> GetMainDescriptions()
    {
        ServiceException? cacheException = null;

        try
        {
            List<string?>? cacheReply = await _cache.GetPrimaryDescriptions();

            if ( cacheReply != null )
                return new Reply<List<string?>?>( cacheReply, true, "Got  Descriptions from cache." );
        }
        catch ( ServiceException e )
        {
            cacheException = e;
        }

        IEnumerable<string?>? descriptionReply = await _repository.GetPrimaryCategoryDescriptions();

        if ( descriptionReply == null )
            return new Reply<List<string?>?>( "Failed to get PrimaryDescrs from repository!" );

        List<string?> descrList = descriptionReply.ToList();
        
        try
        {
            await _cache.SetPrimaryDescriptions( descrList );
        }
        catch ( ServiceException e )
        {
            cacheException = e;
        }

        return new Reply<List<string?>?>( descrList, true, $"Got PrimaryDescriptions from repository. {cacheException?.Message}" );
    }
    public async Task<Reply<string?>> GetDescription( CategoryIdMap? idMap )
    {
        Reply<bool> validationReply = await ValidateCategoryIdMap( idMap );

        if ( !validationReply.Success )
            return new Reply<string?>( "Invalid category!" );

        try
        {
            string? descrReply = await _repository.GetCategoryDescription( idMap!.CategoryId, idMap.Tier );

            return string.IsNullOrEmpty( descrReply )
                ? new Reply<string?>( "Failed to retrieve CategoryDescription!" )
                : new Reply<string?>( descrReply, true, "Retrieved CategoryDescription." );
        }
        catch ( ServiceException e )
        {
            return new Reply<string?>( e.Message );
        }
    }
    public async Task<Reply<CategoryIdMap?>> GetCategoryIdMapFromUrl( string primaryUrl, string? secondaryUrl = null, string? tertiaryUrl = null )
    {
        Reply<CategoryUrlMap?> cacheFetchResponse = await GetCategoryUrlMapFromCache();
        List<string> urlCategories = GetCategoryUrlList( primaryUrl, secondaryUrl, tertiaryUrl );
        
        if ( cacheFetchResponse.Success )
        {
            CategoryIdMap? success = cacheFetchResponse.Data!.GetCategoryIdMapFromUrl( urlCategories );

            return success != null
                ? new Reply<CategoryIdMap?>( success, true, "Successfully got IdMap from cache." )
                : new Reply<CategoryIdMap?>( "Invalid CategoryUrl!" );
        }
        
        Reply<CategoriesResponse?> categoriesResponse = await GetCategoriesResponse();
        if ( !categoriesResponse.Success )
            return new Reply<CategoryIdMap?>( $"CategoriesResponse result: {categoriesResponse.Message} CacheFetch result: {cacheFetchResponse.Message}" );


        CategoryUrlMap urlMap = await MapResponseToUrlMap( categoriesResponse.Data! );
        Reply<bool> cacheSetResponse = await CacheCategoryUrlMap( urlMap );
        
        CategoryIdMap? idTier = urlMap.GetCategoryIdMapFromUrl( urlCategories );
        string returnMessage = $"CategoriesResponse result: {categoriesResponse.Message} CacheFetch result: {cacheFetchResponse.Message} CacheSet result: {cacheSetResponse}";

        return idTier != null
            ? new Reply<CategoryIdMap?>( idTier, true, returnMessage )
            : new Reply<CategoryIdMap?>( returnMessage );
    }
    public async Task<Reply<bool>> ValidateCategoryIdMap( CategoryIdMap? idMap )
    {
        if ( idMap == null )
            return new Reply<bool>( "CategoryIds are null!" );

        Reply<CategoriesResponse?> categoriesReply = await GetCategoriesResponse();
        
        if ( !categoriesReply.Success )
            return new Reply<bool>( $"CategoriesResponse result: {categoriesReply.Message}" );

        return ValidateCategoryIdMap( idMap, categoriesReply.Data! )
            ? new Reply<bool>( true, true, $"Successfully validated CategoryIdMap. {categoriesReply.Message}" )
            : new Reply<bool>( $"Failed to validate CategoryIdMap. {categoriesReply.Message}" );
    }

    async Task<Reply<CategoriesResponse?>> GetCategoriesResponse()
    {
        CategoriesResponse? categoriesResponse;
        ServiceException? cacheFetchException = null;
        
        try
        {
            categoriesResponse = await _cache.GetCategoriesResponse();
            if ( categoriesResponse != null )
                return new Reply<CategoriesResponse?>( categoriesResponse, true, "Got CategoryResponse from cache." );
        }
        catch ( ServiceException e )
        {
            cacheFetchException = e;
        }

        CategoriesModel? repositoryResponse;

        try
        {
            repositoryResponse = await _repository.GetCategories();

            if ( repositoryResponse == null )
                return new Reply<CategoriesResponse?>( $"No categories exist! Cache fetch result: {cacheFetchException?.Message}" );
        }
        catch ( ServiceException e )
        {
            return new Reply<CategoriesResponse?>( e.Message );
        }

        categoriesResponse = await MapModelToResponse( repositoryResponse );
        return new Reply<CategoriesResponse?>( categoriesResponse, true, "Got CategoryResponse from repository." );
    }
    async Task<Reply<bool>> CacheCategoriesResponse( CategoriesResponse response )
    {
        try
        {
            await _cache.SetCategoriesResponse( response );
        }
        catch ( ServiceException e )
        {
            return new Reply<bool>( false, false, $"Failed to cache CategoriesResponse with Exception: {e.Message}" );
        }

        return new Reply<bool>( true, true, "Cached CategoriesResponse." );
    }
    async Task<Reply<CategoryUrlMap?>> GetCategoryUrlMapFromCache()
    {
        CategoryUrlMap? urlMap = null;
        
        try
        {
            urlMap = await _cache.GetUrlMap();
        }
        catch ( ServiceException e )
        {
            return new Reply<CategoryUrlMap?>( null, false, e.Message );
        }

        return new Reply<CategoryUrlMap?>( urlMap, true, "Got CategoryUrlMap from cache." );
    }
    async Task<Reply<bool>> CacheCategoryUrlMap( CategoryUrlMap urlMap )
    {
        try
        {
            await _cache.SetUrlMap( urlMap );
        }
        catch ( ServiceException e )
        {
            return new Reply<bool>( e.Message );
        }

        return new Reply<bool>( true, true, "Cached CategoryUrlMap." );
    }
    
    static async Task<CategoriesResponse> MapModelToResponse( CategoriesModel model )
    {
        var response = new CategoriesResponse();
        Dictionary<short, PrimaryCategoryResponse> primaryDtos = response.PrimaryCategories;
        Dictionary<short, SecondaryCategoryResponse> secondaryDtos = response.SecondaryCategories;
        Dictionary<short, TertiaryCategoryResponse> tertiaryDtos = response.TertiaryCategories;

        await Task.Run( () =>
        {
            foreach ( PrimaryCategory p in model.Primary.Values )
            {
                if ( primaryDtos.ContainsKey( p.PrimaryCategoryId ) )
                    continue;
                
                primaryDtos.Add( p.PrimaryCategoryId, new PrimaryCategoryResponse {
                    Id = p.PrimaryCategoryId,
                    Name = p.Name,
                    Url = p.ApiUrl,
                    ImageUrl = p.ImageUrl
                } );
            }

            foreach ( SecondaryCategory s in model.Secondary.Values )
            {
                if ( secondaryDtos.ContainsKey( s.SecondaryCategoryId ) )
                    continue;
                if ( !primaryDtos.TryGetValue( s.PrimaryCategoryId, out PrimaryCategoryResponse? parent ) )
                    continue;
                if ( parent.ChildCategories.Contains( s.SecondaryCategoryId ) )
                    continue;

                parent.ChildCategories.Add( s.SecondaryCategoryId );
                
                secondaryDtos.Add( s.SecondaryCategoryId, new SecondaryCategoryResponse() {
                    ParentId = parent.Id,
                    Id = s.SecondaryCategoryId,
                    Name = s.Name,
                    Url = s.ApiUrl,
                    ImageUrl = s.ImageUrl
                } );
            }

            foreach ( TertiaryCategory t in model.Tertiary.Values )
            {
                if ( tertiaryDtos.ContainsKey( t.TertiaryCategoryId ) )
                    continue;
                if ( !secondaryDtos.TryGetValue( t.SecondaryCategoryId, out SecondaryCategoryResponse? parent ) )
                    continue;
                if ( parent.ChildCategories.Contains( t.TertiaryCategoryId ) )
                    continue;

                parent.ChildCategories.Add( t.TertiaryCategoryId );

                tertiaryDtos.Add( t.TertiaryCategoryId, new TertiaryCategoryResponse {
                    ParentId = parent.Id,
                    Id = t.TertiaryCategoryId,
                    Name = t.Name,
                    Url = t.ApiUrl,
                    ImageUrl = t.ImageUrl
                } );
            }
        } );

        return response;
    }
    static async Task<CategoryUrlMap> MapResponseToUrlMap( CategoriesResponse response )
    {
        var urlMap = new CategoryUrlMap();
        Dictionary<string, short> primary = urlMap.PrimaryUrlMap;
        Dictionary<string, Dictionary<short, short>> secondary = urlMap.SecondaryUrlMap;
        Dictionary<string, Dictionary<short, short>> tertiary = urlMap.TertiaryUrlMap;

        await Task.Run( () =>
        {
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

                if ( !tertiary.TryGetValue( t.Url, out Dictionary<short, short>? tertiaryMap ) )
                {
                    tertiaryMap = new Dictionary<short, short>();
                    tertiary.Add( t.Url, tertiaryMap );
                }

                tertiaryMap.TryAdd( t.ParentId, t.Id );
            }
        } );
        
        return urlMap;
    }
    static bool ValidateCategoryIdMap( CategoryIdMap map, CategoriesResponse categories )
    {
        return map.Tier switch {
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