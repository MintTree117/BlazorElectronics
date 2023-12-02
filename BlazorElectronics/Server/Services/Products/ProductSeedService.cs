using BlazorElectronics.Server.Models.Products.Seed;
using BlazorElectronics.Server.Repositories.Products;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.SpecLookups;
using BlazorElectronics.Shared.Vendors;

namespace BlazorElectronics.Server.Services.Products;

public sealed class ProductSeedService : ApiService, IProductSeedService
{
    const int SAFETY = 200;
    readonly Random _random = new();

    readonly IProductSeedRepository _repository;
    
    public ProductSeedService( ILogger<ApiService> logger, IProductSeedRepository repository )
        : base( logger )
    {
        _repository = repository;
    }
    
    public async Task<ServiceReply<bool>> SeedProducts( int amount, CategoriesResponse categoriesResponse, SpecLookupsResponse specLookups, VendorsResponse vendors, List<int> users )
    {
        ProductSeedModels seeds = await Task.Run( () => new ProductSeedModels
        {
            Books = GetBookModels( amount, categoriesResponse, specLookups, vendors ),
            Software = GetSoftwareModels( amount, categoriesResponse, specLookups, vendors ),
            Games = GetGamesModels( amount, categoriesResponse, specLookups, vendors ),
            MoviesTv = GetMoviesTvModels( amount, categoriesResponse, specLookups, vendors ),
            Courses = GetCoursesModels( amount, categoriesResponse, specLookups, vendors )
        } );

        try
        {
            bool result = false; // await _repository.SeedProducts( seeds );
            return new ServiceReply<bool>( result );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
    }
    
    // BASE MODEL
    T GetBaseModel<T>( int category, int i, CategoriesResponse categoriesResponse, SpecLookupsResponse lookups, VendorsResponse vendors ) where T : ProductSeedModel, new()
    {
        var model = new T
        {
            VendorId = PickRandomVendor( ( PrimaryCategory ) category, vendors ),
            Title = ProductSeedData.PRODUCT_TITLES[ category ] + " " + i,
            ThumbnailUrl = ProductSeedData.PRODUCT_IMAGES[ category ],
            Price = GetRandomDecimal( ProductSeedData.MIN_PRICE, ProductSeedData.MAX_PRICE ),
            ReleaseDate = GetRandomDate( ProductSeedData.MIN_RELEASE_DATE, ProductSeedData.MAX_RELEASE_DATE ),
            NumberSold = GetRandomInt( 0, ProductSeedData.MAX_NUM_SOLD ),
            HasDrm = GetRandomBoolean(),
            Description = GetRandomDescription( ( PrimaryCategory ) category ),
            Languages = GetRandomSpecLookups( GetLookupMaxIndex( SpecLookupType.Language, lookups ), 8 ),
            MatureContent = GetRandomSpecLookups( GetLookupMaxIndex( SpecLookupType.MatureContent, lookups ), 8 ),
            PrimaryCategoryId = category,
            SecondaryCategoryIds = GetRandomSecondaryCategories( category, categoriesResponse ),
        };

        if ( GetRandomBoolean() )
            model.SalePrice = GetRandomDecimal( ProductSeedData.MIN_PRICE, ( double ) model.Price );
        
        model.TertiaryCategoryIds = GetRandomTertiaryCategories( model.SecondaryCategoryIds, categoriesResponse );
        return model;
    }
    // DESCRIPTIONS
    string GetRandomDescription( PrimaryCategory category )
    {
        return category switch
        {
            PrimaryCategory.BOOKS => ProductSeedData.BOOK_DESCR[ GetRandomInt( 0, ProductSeedData.BOOK_DESCR.Length - 1 ) ],
            PrimaryCategory.SOFTWARE => ProductSeedData.SOFTWARE_DESCR[ GetRandomInt( 0, ProductSeedData.SOFTWARE_DESCR.Length - 1 ) ],
            PrimaryCategory.VIDEOGAMES => ProductSeedData.GAME_DESCR[ GetRandomInt( 0, ProductSeedData.GAME_DESCR.Length - 1 ) ],
            PrimaryCategory.MOVIESTV => ProductSeedData.MOVESTV_DESCR[ GetRandomInt( 0, ProductSeedData.MOVESTV_DESCR.Length - 1 ) ],
            PrimaryCategory.COURSES => ProductSeedData.COURSE_DESCR[ GetRandomInt( 0, ProductSeedData.COURSE_DESCR.Length - 1 ) ],
            _ => string.Empty
        };
    }
    // SPECIFIC MODELS
    List<BookSeedModel> GetBookModels( int amount, CategoriesResponse categoriesResponse, SpecLookupsResponse lookups, VendorsResponse vendors )
    {
        var books = new List<BookSeedModel>();

        for ( int i = 0; i < amount; i++ )
        {
            var book = GetBaseModel<BookSeedModel>( amount, i, categoriesResponse, lookups, vendors );
            book.Author = GetRandomSpec( ProductSeedData.NAMES );
            book.Publisher = GetRandomSpec( ProductSeedData.PUBLISHERS );
            book.ISBN = GetRandomSpec( ProductSeedData.ISBNS );
            book.Pages = GetRandomInt( 0, ProductSeedData.MAX_PAGE_LENGTH );
            book.HasAudio = GetRandomBoolean();
            book.EbookFormats = GetRandomSpecLookups( GetLookupMaxIndex( SpecLookupType.EbookFormat, lookups ), 4 );
            book.Accessibility = GetRandomSpecLookups( GetLookupMaxIndex( SpecLookupType.EbookAccessibility, lookups ), 2 );

            if ( !book.HasAudio ) 
                continue;
            
            book.Narrator = GetRandomSpec( ProductSeedData.NAMES );
            book.AudioLength = GetRandomInt( 0, ProductSeedData.MAX_AUDIO_LENGTH );
            book.AudioBookFormats = GetRandomSpecLookups( GetLookupMaxIndex( SpecLookupType.AudioBookFormat, lookups ), 4 );
        }
        
        return books;
    }
    List<SoftwareSeedSeedModel> GetSoftwareModels( int amount, CategoriesResponse categoriesResponse, SpecLookupsResponse lookups, VendorsResponse vendors )
    {
        var softwares = new List<SoftwareSeedSeedModel>();
        
        for ( int i = 0; i < amount; i++ )
        {
            var software = GetBaseModel<SoftwareSeedSeedModel>( amount, i, categoriesResponse, lookups, vendors );
            software.Version = GetRandomSpec( ProductSeedData.SOFTWARE_VERSIONS );
            software.Developer = GetRandomSpec( ProductSeedData.SOFTWARE_DEVELOPERS );
            software.Dependencies = GetRandomSpecs( ProductSeedData.SOFTWARE_DEPENDENCIES, 5 );
            software.TrialLimitations = GetRandomSpecs( ProductSeedData.SOFTWARE_TRIAL_LIMITATIONS, 5 );
            software.OsRequirements = GetRandomSpecLookups( GetLookupMaxIndex( SpecLookupType.OsRequirement, lookups ), 3 );
            software.SoftwareLanguages = GetRandomSpecLookups( GetLookupMaxIndex( SpecLookupType.SoftwareLanguage, lookups ), 4 );
        }
        
        return softwares;
    }
    List<GamesSeedModel> GetGamesModels( int amount, CategoriesResponse categoriesResponse, SpecLookupsResponse lookups, VendorsResponse vendors )
    {
        var games = new List<GamesSeedModel>();

        for ( int i = 0; i < amount; i++ )
        {
            var game = GetBaseModel<GamesSeedModel>( amount, i, categoriesResponse, lookups, vendors );
            game.Developer = GetRandomSpec( ProductSeedData.GAME_DEVELOPERS );
            game.HasMultiplayer = GetRandomBoolean();
            game.HasOfflineMode = GetRandomBoolean();
            game.HasInGamePurchases = GetRandomBoolean();
            game.HasControllerSupport = GetRandomBoolean();
            game.FileSizeMb = GetRandomInt( ProductSeedData.MIN_FILE_SIZE, ProductSeedData.MAX_FILE_SIZE );
            game.OsRequirements = GetRandomSpecLookups( GetLookupMaxIndex( SpecLookupType.OsRequirement, lookups ), 3 );

            if ( game.HasMultiplayer )
            {
                game.MultiplayerDetails = GetRandomSpecLookups( GetLookupMaxIndex( SpecLookupType.MultiplayerDetail, lookups ), 6 );
            }
        }

        return games;
    }
    List<MoviesTvSeedModel> GetMoviesTvModels( int amount, CategoriesResponse categoriesResponse, SpecLookupsResponse lookups, VendorsResponse vendors )
    {
        var moviesTv = new List<MoviesTvSeedModel>();

        for ( int i = 0; i < amount; i++ )
        {
            var movieTv = GetBaseModel<MoviesTvSeedModel>( amount, i, categoriesResponse, lookups, vendors );
            movieTv.Director = GetRandomSpec( ProductSeedData.DIRECTORS );
            movieTv.Cast = GetRandomSpecs( ProductSeedData.ACTORS, 10 );
            movieTv.RuntimeMinutes = GetRandomInt( 0, ProductSeedData.MAX_RUNTIME );
            movieTv.Episodes = GetRandomInt( 0, ProductSeedData.MAX_EPISODES );
            movieTv.HasSubtitles = GetRandomBoolean();
            movieTv.VideoFormats = GetRandomSpecLookups( GetLookupMaxIndex( SpecLookupType.VideoFormat, lookups ), 3 );
            movieTv.Accessibility = GetRandomSpecLookups( GetLookupMaxIndex( SpecLookupType.VideoAccessibility, lookups ), 3 );
            movieTv.Awards = GetRandomSpecLookups( GetLookupMaxIndex( SpecLookupType.MoviesTvAward, lookups ), 5 );
        }

        return moviesTv;
    }
    List<CourseSeedModel> GetCoursesModels( int amount, CategoriesResponse categoriesResponse, SpecLookupsResponse lookups, VendorsResponse vendors )
    {
        var courses = new List<CourseSeedModel>();

        for ( int i = 0; i < amount; i++ )
        {
            var course = GetBaseModel<CourseSeedModel>( amount, i, categoriesResponse, lookups, vendors );
            course.Instructors = GetRandomSpecs( ProductSeedData.NAMES, 3 );
            course.Requirements = GetRandomSpecs( ProductSeedData.COURSE_REQUIREMENTS, 5 );
            course.DurationWeeks = GetRandomInt( 1, ProductSeedData.MAX_COURSE_DURATION );
            course.HasSubtitles = GetRandomBoolean();
            course.Certifications = GetRandomSpecLookups( GetLookupMaxIndex( SpecLookupType.CourseCertificate, lookups ), 3 );
            course.VideoAccessibility = GetRandomSpecLookups( GetLookupMaxIndex( SpecLookupType.VideoAccessibility, lookups ), 3 );
        }

        return courses;
    }
    // CATEGORY
    List<int> GetRandomSecondaryCategories( int primaryCategory, CategoriesResponse categoryResponseData )
    {
        /*CategoryResponse? response = categoryResponseData.Primary.FirstOrDefault( p => p.Id == primaryCategory );

        return response is null 
            ? new List<int>() 
            : PickRandomCategories( response ).ToList();*/
        return new List<int>();
    }
    List<int> GetRandomTertiaryCategories( List<int> secondaryCategories, CategoriesResponse categoryResponseData )
    {
        var tertiaryCategories = new List<int>();
        var secondary = new List<CategoryResponse>();

        /*foreach ( int secondaryCategory in secondaryCategories )
        {
            CategoryResponse? r = categoryResponseData.Secondary.FirstOrDefault( s => s.Id == secondaryCategory );

            if ( r is not null )
                secondary.Add( r );
        }

        foreach ( CategoryResponse r in secondary )
        {
            IEnumerable<int> picked = PickRandomCategories( r );

            foreach ( int i in picked )
            {
                if ( !tertiaryCategories.Contains( i ) )
                    tertiaryCategories.Add( i );
            }
        }*/
        
        return tertiaryCategories;
    }
    IEnumerable<int> PickRandomCategories( CategoryResponse response )
    {
        var picked = new HashSet<int>();

        int maxIndex = response.Children.Count - 1;
        int amount = GetRandomInt( 1, ProductSeedData.MAX_CATEGORIES );
        int count = 0;

        while ( picked.Count < amount )
        {
            int i = GetRandomInt( 0, maxIndex );

            if ( picked.Contains( i ) )
            {
                count++;
                if ( count > SAFETY )
                    break;
            }

            //picked.Add( response.Children[ i ] );
        }

        return picked.ToList();
    }
    // SPEC LOOKUP
    List<int> GetRandomSpecLookups( int maxIndex, int maxAmount )
    {
        var alreadyPicked = new HashSet<int>();
        
        int amount = GetRandomInt( 0, maxAmount );
        int count = 0;
        
        while ( alreadyPicked.Count < amount )
        {
            int i = GetRandomInt( 0, maxIndex );

            if ( alreadyPicked.Contains( i ) )
            {
                count++;
                
                if ( count > SAFETY )
                    break;
            }
            
            alreadyPicked.Add( i );
        }

        return alreadyPicked.ToList();
    }
    int GetLookupMaxIndex( SpecLookupType type, SpecLookupsResponse lookups )
    {
        return lookups.ResponsesBySpecId[ ( int ) type ].Values.Count - 1;
    }
    // SPEC
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
    // VENDORS
    int PickRandomVendor( PrimaryCategory category, VendorsResponse vendors )
    {
        List<int> vendorIds = vendors.VendorIdsByCategory[ category ];
        return vendorIds[ GetRandomInt( 0, vendorIds.Count - 1 ) ];
    }
    // RANDOM VALUES
    DateTime GetRandomDate( DateTime min, DateTime max )
    {
        // Convert to ticks
        long range = max.Ticks - min.Ticks;

        // Create a random range between them
        long randomTicks = ( long ) ( _random.NextDouble() * range ) + ProductSeedData.MIN_RELEASE_DATE.Ticks;

        // Convert back to DateTime
        return new DateTime( randomTicks );
    }
    int GetRandomInt( int min, int max )
    {
        // Generate a random double number between 0 and 1
        double randomDouble = _random.NextDouble();

        // Adjust the range of the double number
        double range = ( double ) ( ProductSeedData.MAX_NUM_SOLD );
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
    decimal GetRandomDecimal( double min, double max )
    {
        double range = ( max - min );
        double sample = _random.NextDouble();
        double scaled = ( sample * range ) + min;
        return ( decimal ) scaled;
    }
}