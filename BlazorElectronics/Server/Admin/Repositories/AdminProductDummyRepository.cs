using BlazorElectronics.Server.Admin.Models.Products;
using BlazorElectronics.Server.Admin.Models.Variants;
using BlazorElectronics.Server.Admin.Services;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Dtos.Categories;
using BlazorElectronics.Server.Dtos.SpecLookups;
using BlazorElectronics.Server.Repositories;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Server.Admin.Repositories;

public sealed class AdminProductDummyRepository : DapperRepository, IAdminProductDummyInsertRepository
{
    int MAX_CATEGORIES = 3;
    
    readonly Random _random = new();
    
    public AdminProductDummyRepository( DapperContext dapperContext )
        : base( dapperContext ) { }
    
    public async Task<bool> Insert( int amount, CachedCategories cachedCategories, VariantsModel variants, CachedSpecLookupData specLookups )
    {
        ProductModels products = await GetProducts( amount, cachedCategories, variants, specLookups );
        return false;
    }

    async Task<ProductModels> GetProducts( int amount, CachedCategories cachedCategories, VariantsModel variants, CachedSpecLookupData specLookups )
    {
        return await Task.Run( () => new ProductModels
        {
            Books = GetBookModels( amount, cachedCategories, variants, specLookups ),
            Software = GetSoftwareModels( amount, cachedCategories, variants, specLookups ),
            Games = GetGamesModels( amount, cachedCategories, specLookups ),
            MoviesTv = GetMoviesTvModels( amount, cachedCategories, specLookups ),
            Courses = GetCoursesModels( amount, cachedCategories, specLookups )
        } );
    }
    
    T GetBaseModel<T>( int category, int i, CachedCategories cachedCategories, CachedSpecLookupData specLookups ) where T : AdminProductModel, new()
    {
        var model = new T
        {
            Title = AdminProductData.PRODUCT_TITLES[ category ] + " " + i,
            ThumbnailUrl = AdminProductData.PRODUCT_IMAGES[ category ],
            Rating = GetRandomFloat( 0, AdminProductData.MAX_RATING ),
            ReleaseDate = GetRandomDate(),
            NumberSold = GetRandomInt( 0, AdminProductData.MAX_NUM_SOLD ),
            HasDrm = GetRandomBoolean(),
            HasSale = GetRandomBoolean(),
            //Languages = GetRandomSpecLookups( specLookups.Languages ),
            //MatureContent = GetRandomSpecLookups( specLookups.MatureContent ),
            PrimaryCategoryId = category,
            SecondaryCategoryIds = GetRandomSecondaryCategories( category, cachedCategories ),
        };

        model.TertiaryCategoryIds = GetRandomTertiaryCategories( model.SecondaryCategoryIds, cachedCategories );

        return model;
    }
    List<AdminBookModel> GetBookModels( int amount, CachedCategories cachedCategories, VariantsModel variants, CachedSpecLookupData specLookups )
    {
        var books = new List<AdminBookModel>();

        for ( int i = 0; i < amount; i++ )
        {
            var book = GetBaseModel<AdminBookModel>( amount, i, cachedCategories, specLookups );
            book.Author = GetRandomSpec( AdminProductData.NAMES );
            book.Publisher = GetRandomSpec( AdminProductData.PUBLISHERS );
            book.ISBN = GetRandomSpec( AdminProductData.ISBNS );
            book.Pages = GetRandomInt( 0, AdminProductData.MAX_PAGE_LENGTH );
            book.HasAudio = GetRandomBoolean();
            //book.EbookFormats = GetRandomSpecLookups( specLookups.EbookFormats );

            book.Variants.Add( GetRandomVariant( ( int ) VariantsBooks.EBook , book.HasSale ) );
            
            if ( !book.HasAudio ) 
                continue;
            
            book.Narrator = GetRandomSpec( AdminProductData.NAMES );
            book.AudioLength = GetRandomInt( 0, AdminProductData.MAX_AUDIO_LENGTH );
            //book.AudioBookFormats = GetRandomSpecLookups( specLookups.AudioBookFormats );

            book.Variants.Add( GetRandomVariant( ( int ) VariantsBooks.AudioBook, book.HasSale ) );
        }
        
        return books;
    }
    List<AdminSoftwareModel> GetSoftwareModels( int amount, CachedCategories cachedCategories, VariantsModel variants, CachedSpecLookupData specLookups )
    {
        var softwares = new List<AdminSoftwareModel>();
        
        for ( int i = 0; i < amount; i++ )
        {
            var software = GetBaseModel<AdminSoftwareModel>( amount, i, cachedCategories, specLookups );
            software.Version = GetRandomSpec( AdminProductData.SOFTWARE_VERSIONS );
            software.Developer = GetRandomSpec( AdminProductData.SOFTWARE_DEVELOPERS );
            software.Dependencies = GetRandomSpecs( AdminProductData.SOFTWARE_DEPENDENCIES, 5 );
            software.TrialLimitations = GetRandomSpecs( AdminProductData.SOFTWARE_TRIAL_LIMITATIONS, 5 );
            //software.OsRequirements = GetRandomSpecLookups( specLookups.OsRequirements );

            software.Variants.Add( GetRandomVariant( ( int ) VariantsSoftware.Personal, software.HasSale ) );
            if ( GetRandomBoolean() )
                software.Variants.Add( GetRandomVariant( ( int ) VariantsSoftware.Commercial, software.HasSale ) );
        }
        
        return softwares;
    }
    List<AdminGamesModel> GetGamesModels( int amount, CachedCategories cachedCategories, CachedSpecLookupData specLookups )
    {
        var games = new List<AdminGamesModel>();

        for ( int i = 0; i < amount; i++ )
        {
            var game = GetBaseModel<AdminGamesModel>( amount, i, cachedCategories, specLookups );
            game.Developer = GetRandomSpec( AdminProductData.GAME_DEVELOPERS );
            game.HasMultiplayer = GetRandomBoolean();
            game.HasOfflineMode = GetRandomBoolean();
            game.HasInGamePurchases = GetRandomBoolean();
            game.HasControllerSupport = GetRandomBoolean();

            if ( game.HasMultiplayer )
            {
                //game.MultiplayerDetails = GetRandomSpecLookups( specLookups.MultiplayerDetails );
            }

            game.Variants.Add( GetRandomVariant( 1, game.HasSale ) );
        }

        return games;
    }
    List<AdminMoviesTvModel> GetMoviesTvModels( int amount, CachedCategories cachedCategories, CachedSpecLookupData specLookups )
    {
        var moviesTv = new List<AdminMoviesTvModel>();

        for ( int i = 0; i < amount; i++ )
        {
            var movieTv = GetBaseModel<AdminMoviesTvModel>( amount, i, cachedCategories, specLookups );
            movieTv.Director = GetRandomSpec( AdminProductData.DIRECTORS );
            movieTv.Cast = GetRandomSpecs( AdminProductData.ACTORS, 10 );
            movieTv.RuntimeMinutes = GetRandomInt( 0, AdminProductData.MAX_RUNTIME );
            movieTv.Episodes = GetRandomInt( 0, AdminProductData.MAX_EPISODES );
            movieTv.HasSubtitles = GetRandomBoolean();
            //movieTv.Awards = GetRandomSpecLookups( specLookups.MoviesTvAwards );

            movieTv.Variants.Add( GetRandomVariant( 1, movieTv.HasSale ) );
        }

        return moviesTv;
    }
    List<AdminCourseModel> GetCoursesModels( int amount, CachedCategories cachedCategories, CachedSpecLookupData specLookups )
    {
        var courses = new List<AdminCourseModel>();

        for ( int i = 0; i < amount; i++ )
        {
            var course = GetBaseModel<AdminCourseModel>( amount, i, cachedCategories, specLookups );
            course.Instructors = GetRandomSpecs( AdminProductData.NAMES, 3 );
            course.Requirements = GetRandomSpecs( AdminProductData.COURSE_REQUIREMENTS, 5 );
            course.DurationWeeks = GetRandomInt( 1, AdminProductData.MAX_COURSE_DURATION );
            course.HasSubtitles = GetRandomBoolean();

            course.Variants.Add( GetRandomVariant( 1, course.HasSale ) );
        }

        return courses;
    }
    
    // UTILITY
    List<int> GetRandomSecondaryCategories( int primaryCategory, CachedCategories cachedCategoryData )
    {
        int pcId = 0;// cachedCategoryData.PrimaryIds[ primaryCategory ];
        PrimaryCategoryResponse response = cachedCategoryData.PrimaryResponses[ pcId ];
        
        var picked = new HashSet<int>();
        
        int maxIndex = response.ChildCategories.Count - 1;
        int amount = Math.Min( GetRandomInt( 0, maxIndex ), MAX_CATEGORIES );
        const int safety = 200;
        int count = 0;
        
        while ( picked.Count < amount )
        {
            int i = GetRandomInt( 0, maxIndex );

            if ( picked.Contains( i ) )
            {
                count++;
                if ( count > safety )
                    break;
            }
            
            picked.Add( response.ChildCategories[ i ] );
        }
        
        return picked.ToList();
    }
    List<int> GetRandomTertiaryCategories( List<int> secondaryCategories, CachedCategories cachedCategoryData )
    {
        var tertiaryCategories = new List<int>();

        foreach ( int secondaryCategory in secondaryCategories )
        {
            int scId = 0; //cachedCategoryData.SecondaryIds[ secondaryCategory ];
            SecondaryCategoryResponse response = cachedCategoryData.SecondaryResponses[ scId ];

            var picked = new HashSet<int>();

            int maxIndex = response.ChildCategories.Count - 1;
            int amount = Math.Min( GetRandomInt( 0, maxIndex ), MAX_CATEGORIES );
            const int safety = 200;
            int count = 0;

            while ( picked.Count < amount )
            {
                int i = GetRandomInt( 0, maxIndex );

                if ( picked.Contains( i ) )
                {
                    count++;
                    if ( count > safety )
                        break;
                }

                picked.Add( response.ChildCategories[ i ] );
            }
            
            tertiaryCategories.AddRange( picked.ToList() );
        }
        
        return tertiaryCategories;
    }
    List<int> GetRandomSpecLookups( IReadOnlyList<CachedSpecLookupData> lookups )
    {
        var alreadyPicked = new HashSet<int>();

        int maxIndex = lookups.Count - 1;
        int amount = GetRandomInt( 0, maxIndex );
        int safety = 200;
        int count = 0;
        while ( alreadyPicked.Count < amount )
        {
            int i = GetRandomInt( 0, maxIndex );

            if ( alreadyPicked.Contains( i ) )
            {
                count++;
                if ( count > safety )
                    break;
            }
            
            alreadyPicked.Add( i );
        }

        return alreadyPicked.ToList();
    }
    VariantModel GetRandomVariant( int id, bool onSale )
    {
        decimal price = GetRandomDecimal( 0, AdminProductData.MAX_PRICE );

        return new VariantModel()
        {
            VariantValueId = ( int ) VariantsBooks.EBook,
            VariantPrice = price,
            VariantSalePrice = onSale ? GetRandomDecimal( 0, price ) : null
        };
    }
    string GetRandomSpec( IReadOnlyList<string> list )
    {
        int index = GetRandomInt( 0, list.Count - 1 );
        return list[ index ];
    }
    string GetRandomSpecs( IReadOnlyList<string> list, int max )
    {
        var specs = new Dictionary<int, string>();

        int amount = GetRandomInt( 0, Math.Min( list.Count - 1, max ) );
        int count = 0;

        while ( count < amount )
        {
            int i = GetRandomInt( 0, list.Count - 1 );

            if ( specs.ContainsKey( i ) )
                continue;

            specs.Add( i, list[ i ] );
            count++;
        }

        return string.Join( ",", specs.Values );
    }
    
    DateTime GetRandomDate()
    {
        // Convert to ticks
        long range = AdminProductData.MAX_RELEASE_DATE.Ticks - AdminProductData.MIN_RELEASE_DATE.Ticks;

        // Create a random range between them
        long randomTicks = ( long ) ( _random.NextDouble() * range ) + AdminProductData.MIN_RELEASE_DATE.Ticks;

        // Convert back to DateTime
        return new DateTime( randomTicks );
    }
    int GetRandomInt( int min, int max )
    {
        // Generate a random double number between 0 and 1
        double randomDouble = _random.NextDouble();

        // Adjust the range of the double number
        double range = ( double ) ( AdminProductData.MAX_NUM_SOLD );
        double randomInRange = ( randomDouble * range );

        // Convert to decimal and return
        return ( int ) randomInRange;
    }
    float GetRandomFloat( float minValue, float maxValue )
    {
        // Check if the arguments are valid
        if ( minValue >= maxValue )
        {
            throw new ArgumentOutOfRangeException( nameof( minValue ), "minValue must be less than maxValue" );
        }

        // Generate a random double, cast it to float, and adjust the range
        float range = maxValue - minValue;
        float randomFloat = ( float ) _random.NextDouble(); // This will be between 0.0 and 1.0
        return ( randomFloat * range ) + minValue;
    }
    bool GetRandomBoolean()
    {
        // Generate a random integer (0 or 1) and convert it to a boolean
        return _random.Next( 2 ) == 1;
    }
    decimal GetRandomDecimal( decimal min, decimal max )
    {
        return 0;
    }

    class ProductModels
    {
        public List<AdminBookModel> Books { get; set; } = new();
        public List<AdminSoftwareModel> Software { get; set; } = new();
        public List<AdminGamesModel> Games { get; set; } = new();
        public List<AdminMoviesTvModel> MoviesTv { get; set; } = new();
        public List<AdminCourseModel> Courses { get; set; } = new();
    }
}