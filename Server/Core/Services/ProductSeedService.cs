using System.Xml.Linq;
using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Server.Core.Models.Products;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.Specs;
using BlazorElectronics.Shared.Vendors;

namespace BlazorElectronics.Server.Core.Services;

public sealed class ProductSeedService : ApiService, IProductSeedService
{
    const int SAFETY = 200;
    Random _random = new();

    readonly IProductRepository _productRepository;
    readonly IReviewRepository _reviewRepository;

    public ProductSeedService( ILogger<ApiService> logger, IProductRepository productRepository, IReviewRepository reviewRepository )
        : base( logger )
    {
        _productRepository = productRepository;
        _reviewRepository = reviewRepository;
    }
    
    public async Task<ServiceReply<bool>> SeedProducts( int amount, CategoryData categories, LookupSpecsDto lookups, VendorsDto vendors)
    {
        _random = new Random();
        
        List<ProductEditModel> models = new();

        await Task.Run( () =>
        {
            for ( int primaryCategory = 1; primaryCategory <= 5; primaryCategory++ )
            {
                for ( int i = 0; i < amount; i++ )
                {
                    models.Add( GetSeedModel( primaryCategory, i, categories, lookups, vendors ) );
                }
            }
        } );

        foreach ( ProductEditModel m in models )
        {
            try
            {
                int productId = await _productRepository.Insert( m );

                if ( productId <= 0 )
                    return new ServiceReply<bool>( false );
            }
            catch ( RepositoryException e )
            {
                Logger.LogError( e.Message, e );
                return new ServiceReply<bool>( ServiceErrorType.ServerError, "Failed to seed products!" );
            }
        }

        return new ServiceReply<bool>( true );
    }
    
    // BASE MODEL
    ProductEditModel GetSeedModel( int primaryCategory, int i, CategoryData categoryData, LookupSpecsDto lookups, VendorsDto vendors )
    {
        ProductEditModel editModel = new()
        {
            VendorId = PickRandomVendor( primaryCategory, vendors ),
            Title = SeedData.PRODUCT_TITLES[ primaryCategory - 1 ] + " " + i,
            ThumbnailUrl = $"/Images/{SeedData.PRODUCT_THUMBNAILS[ primaryCategory - 1 ]}",
            ReleaseDate = GetRandomDate( SeedData.MIN_RELEASE_DATE, SeedData.MAX_RELEASE_DATE ),
            IsFeatured = ReturnTrue20Percent(),
            Images = GetProductImages( primaryCategory ),
            Price = GetRandomDecimal( SeedData.MIN_PRICE, SeedData.MAX_PRICE ),
            NumberSold = GetRandomInt( 0, SeedData.MAX_NUM_SOLD ),
            Description = GetRandomDescription( primaryCategory ),
            
        };

        if ( GetRandomBoolean() )
            editModel.SalePrice = GetRandomDecimal( SeedData.MIN_PRICE, ( double ) editModel.Price );

        editModel.Categories.Add( primaryCategory );
        editModel.Categories.AddRange( GetRandomCategories( primaryCategory, new List<int>(), categoryData ) );
        
        GetRandomLookups( lookups.GlobalSpecIds, lookups )
            .ToList()
            .ForEach( x => editModel.SpecLookups[ x.Key ] = x.Value );

        GetRandomLookups( lookups.SpecIdsByCategory[ primaryCategory ], lookups )
            .ToList()
            .ForEach( x => editModel.SpecLookups[ x.Key ] = x.Value );

        editModel.XmlSpecs = primaryCategory switch
        {
            1 => GetBookXml(),
            2 => GetSoftwareXml(),
            3 => GetGameXml(),
            4 => GetMoviesTvXml(),
            5 => GetCoursesXml(),
            _ => throw new ArgumentOutOfRangeException( nameof( primaryCategory ), primaryCategory, null )
        };

        return editModel;
    }
    // IMAGES
    List<string> GetProductImages( int category )
    {
        string imageName = category switch
        {
            1 => "Book",
            2 => "Software",
            3 => "Game",
            4 => "Video",
            5 => "Course",
            _ => throw new ArgumentOutOfRangeException( nameof( category ), category, null )
        };
        
        List<string> images = SeedData.PRODUCT_IMAGES[ category - 1 ].ToList();
        for ( int i = 0; i < images.Count; i++ )
        {
            images[ i ] = $"/Images/{imageName}{i+1}.jpg";
        }
        return images;
    }
    // DESCRIPTIONS
    string GetRandomDescription( int category )
    {
        return category switch
        {
            1 => SeedData.BOOK_DESCR[ GetRandomInt( 0, SeedData.BOOK_DESCR.Length - 1 ) ],
            2 => SeedData.SOFTWARE_DESCR[ GetRandomInt( 0, SeedData.SOFTWARE_DESCR.Length - 1 ) ],
            3 => SeedData.GAME_DESCR[ GetRandomInt( 0, SeedData.GAME_DESCR.Length - 1 ) ],
            4 => SeedData.MOVESTV_DESCR[ GetRandomInt( 0, SeedData.MOVESTV_DESCR.Length - 1 ) ],
            5 => SeedData.COURSE_DESCR[ GetRandomInt( 0, SeedData.COURSE_DESCR.Length - 1 ) ],
            _ => string.Empty
        };
    }
    // XML
    string GetBookXml()
    {
        var author = new XElement( "Author", GetRandomItem( SeedData.NAMES ) );
        var publisher = new XElement( "Publisher", GetRandomItem( SeedData.PUBLISHERS ) );
        var isbn = new XElement( "ISBN", GetRandomItem( SeedData.ISBNS ) );
        var pages = new XElement( "Pages", GetRandomInt( 0, SeedData.MAX_PAGE_LENGTH ).ToString() );
        
        var root = new XElement( "RootElement" );
        root.Add( author );
        root.Add( publisher );
        root.Add( isbn );
        root.Add( pages );

        if ( !GetRandomBoolean() ) 
            return root.ToString();
        
        var narrator = new XElement( "Narrator", GetRandomItem( SeedData.NAMES ) );
        var audioLength = new XElement( "AudioLength", GetRandomInt( 0, SeedData.MAX_AUDIO_LENGTH ).ToString() );
        root.Add( narrator );
        root.Add( audioLength );

        return root.ToString();
    }
    string GetSoftwareXml()
    {
        var version = new XElement( "Version", GetRandomItem( SeedData.SOFTWARE_VERSIONS ) );
        var developer = new XElement( "Developer", GetRandomItem( SeedData.SOFTWARE_DEVELOPERS ) );
        var dependencies = new XElement( "Dependencies", GetRandomItems( SeedData.SOFTWARE_DEPENDENCIES, 5 ) );
        var trialLimitations = new XElement( "TrialLimitations", GetRandomItem( SeedData.SOFTWARE_TRIAL_LIMITATIONS ) );

        var root = new XElement( "RootElement" );
        root.Add( version );
        root.Add( developer );
        root.Add( dependencies );
        root.Add( trialLimitations );

        return root.ToString();
    }
    string GetGameXml()
    {
        var developer = new XElement( "Developer", GetRandomItem( SeedData.GAME_DEVELOPERS ) );
        var hasOfflineMode = new XElement( "HasOfflineMode", GetRandomBoolean().ToString() );
        var hasInGamePurchases = new XElement( "HasInGamePurchases", GetRandomBoolean().ToString() );
        var hasControllerSupport = new XElement( "HasControllerSupport", GetRandomBoolean().ToString() );
        var fileSize = new XElement( "FileSizeMb", GetRandomInt( SeedData.MIN_FILE_SIZE, SeedData.MAX_FILE_SIZE ).ToString() );

        var root = new XElement( "RootElement" );
        root.Add( developer );
        root.Add( hasOfflineMode );
        root.Add( hasInGamePurchases );
        root.Add( hasControllerSupport );
        root.Add( fileSize );
        
        return root.ToString();
    }
    string GetMoviesTvXml()
    {
        var director = new XElement( "Director", GetRandomItem( SeedData.DIRECTORS ) );
        var cast = new XElement( "Cast", GetRandomItems( SeedData.NAMES, 10) );
        var runtimeMinutes = new XElement( "RuntimeMinutes", GetRandomInt( 1, SeedData.MAX_RUNTIME ).ToString() );
        var episodes = new XElement( "Episodes", GetRandomInt( 1, SeedData.MAX_EPISODES ).ToString() );
        var hasSubtitles = new XElement( "HasSubtitles", GetRandomBoolean().ToString() );

        var root = new XElement( "RootElement" );
        root.Add( director );
        root.Add( cast );
        root.Add( runtimeMinutes );
        root.Add( episodes );
        root.Add( hasSubtitles );

        return root.ToString();
    }
    string GetCoursesXml()
    {
        var instructors = new XElement( "Instructors", GetRandomItems( SeedData.NAMES, 3 ) );
        var requirements = new XElement( "Requirements", GetRandomItems( SeedData.COURSE_REQUIREMENTS, 5 ) );
        var durationWeeks = new XElement( "DurationWeeks", GetRandomInt( 1, SeedData.MAX_COURSE_DURATION ).ToString() );
        var hasSubtitles = new XElement( "HasSubtitles", GetRandomBoolean().ToString() );

        var root = new XElement( "RootElement" );
        root.Add( instructors );
        root.Add( requirements );
        root.Add( durationWeeks );
        root.Add( hasSubtitles );
        
        return root.ToString();
    }
    // IMAGES
    // CATEGORY
    IEnumerable<int> GetRandomCategories( int currentCategory, List<int> pickedCategories, CategoryData categoryData )
    {
        CategoryFullDto categoryFullDto = categoryData.CategoriesById[ currentCategory ];

        if ( categoryFullDto.Children.Count <= 0 )
            return new List<int>(); // Return an empty list instead of the original one

        List<int> subcategories = PickRandomCategories( categoryFullDto ).ToList();
        List<int> newPickedCategories = new List<int>( subcategories ); // Create a new list for this call

        foreach ( int category in subcategories )
        {
            // Merge the results from the recursive calls into the new list
            newPickedCategories.AddRange( GetRandomCategories( category, newPickedCategories, categoryData ) );
        }

        return newPickedCategories.Distinct(); // Remove duplicates before returning
    }
    IEnumerable<int> PickRandomCategories( CategoryFullDto categoryFull )
    {
        var picked = new HashSet<int>();

        int maxIndex = categoryFull.Children.Count - 1;
        int amount = GetRandomInt( 1, SeedData.MAX_CATEGORIES );
        int count = 0;

        while ( picked.Count < amount )
        {
            int i = GetRandomInt( 0, maxIndex );
            int index = categoryFull.Children[ i ].CategoryId;

            if ( picked.Contains( index ) )
            {
                count++;
                if ( count > SAFETY )
                    break;
                
                continue;
            }

            picked.Add( index );
        }
        
        return picked.ToList();
    }
    // SPEC LOOKUP
    Dictionary<int, List<int>> GetRandomLookups( List<int> ids, LookupSpecsDto lookups )
    {
        Dictionary<int, List<int>> specs = new();

        foreach ( int i in ids )
        {
            LookupSpec response = lookups.SpecsById[ i ];
            List<int> valueIds = PickRandomSpecValueIds( response.Values.Count );
            specs.Add( i, valueIds );
        }

        return specs;
    }
    List<int> PickRandomSpecValueIds( int listCount )
    {
        var alreadyPicked = new HashSet<int>();
        
        int amount = GetRandomInt( 0, listCount );
        int count = 0;
        
        while ( alreadyPicked.Count < amount )
        {
            int i = GetRandomInt( 0, listCount - 1 );

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
    // SPEC
    string GetRandomItem( IReadOnlyList<string> list )
    {
        int index = GetRandomInt( 0, list.Count - 1 );
        return list[ index ];
    }
    string GetRandomItems( IReadOnlyList<string> list, int max )
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
    int PickRandomVendor( int category, VendorsDto vendors )
    {
        return vendors.VendorIdsByCategory.TryGetValue( category, out List<int>? ids ) 
            ? ids[ GetRandomInt( 0, ids.Count - 1 ) ]
            : 0;
    }
    // RANDOM VALUES
    DateTime GetRandomDate( DateTime min, DateTime max )
    {
        // Convert to ticks
        long range = max.Ticks - min.Ticks;

        // Create a random range between them
        long randomTicks = ( long ) ( _random.NextDouble() * range ) + SeedData.MIN_RELEASE_DATE.Ticks;

        // Convert back to DateTime
        return new DateTime( randomTicks );
    }
    int GetRandomInt( int min, int max )
    {
        if ( min >= max )
            throw new ArgumentOutOfRangeException( nameof( min ), "min must be less than max" );

        return _random.Next( min, max );
    }
    bool ReturnTrue20Percent()
    {
        int randomNumber = _random.Next( 100 );
        return randomNumber < 20;
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