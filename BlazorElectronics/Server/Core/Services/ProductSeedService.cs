using System.Xml.Linq;
using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Server.Core.Models.Products;
using BlazorElectronics.Server.Data;
using BlazorElectronics.Server.Services;
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
    readonly IProductReviewRepository _reviewRepository;

    public ProductSeedService( ILogger<ApiService> logger, IProductRepository productRepository, IProductReviewRepository reviewRepository )
        : base( logger )
    {
        _productRepository = productRepository;
        _reviewRepository = reviewRepository;
    }
    
    public async Task<ServiceReply<bool>> SeedProducts( int amount, CategoryData categories, SpecsResponse lookups, VendorsResponse vendors, List<int> users )
    {
        _random = new Random();
        
        List<ProductEditModel> models = new();

        await Task.Run( () =>
        {
            for ( int primaryCategory = 1; primaryCategory <= 5; primaryCategory++ )
            {
                for ( int i = 0; i < amount; i++ )
                {
                    models.Add( GetSeedModel( primaryCategory, i, categories, lookups, vendors, users ) );
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

                foreach ( ProductReviewModel r in m.Reviews )
                {
                    //r.ProductId = productId;
                    //int reviewResult = await _reviewRepository.Insert( r );
                    
                    //if ( reviewResult <= 0 )
                        //return new ServiceReply<bool>( false );
                }
                
                //await _productRepository.UpdateReviewCount();
                //await _productRepository.UpdateRatings();
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
    ProductEditModel GetSeedModel( int primaryCategory, int i, CategoryData categoryData, SpecsResponse lookups, VendorsResponse vendors, IReadOnlyList<int> users )
    {
        ProductEditModel editModel = new()
        {
            VendorId = PickRandomVendor( primaryCategory, vendors ),
            Title = ProductSeedData.PRODUCT_TITLES[ primaryCategory - 1 ] + " " + i,
            ThumbnailUrl = $"/Images/{ProductSeedData.PRODUCT_THUMBNAILS[ primaryCategory - 1 ]}",
            Images = ProductSeedData.PRODUCT_IMAGES[ primaryCategory - 1 ].ToList(),
            Price = GetRandomDecimal( ProductSeedData.MIN_PRICE, ProductSeedData.MAX_PRICE ),
            ReleaseDate = GetRandomDate( ProductSeedData.MIN_RELEASE_DATE, ProductSeedData.MAX_RELEASE_DATE ),
            NumberSold = GetRandomInt( 0, ProductSeedData.MAX_NUM_SOLD ),
            Description = GetRandomDescription( primaryCategory ),
        };

        if ( GetRandomBoolean() )
            editModel.SalePrice = GetRandomDecimal( ProductSeedData.MIN_PRICE, ( double ) editModel.Price );

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

        editModel.Reviews = GetRandomReviews( primaryCategory, users );

        return editModel;
    }
    // DESCRIPTIONS
    string GetRandomDescription( int category )
    {
        return category switch
        {
            1 => ProductSeedData.BOOK_DESCR[ GetRandomInt( 0, ProductSeedData.BOOK_DESCR.Length - 1 ) ],
            2 => ProductSeedData.SOFTWARE_DESCR[ GetRandomInt( 0, ProductSeedData.SOFTWARE_DESCR.Length - 1 ) ],
            3 => ProductSeedData.GAME_DESCR[ GetRandomInt( 0, ProductSeedData.GAME_DESCR.Length - 1 ) ],
            4 => ProductSeedData.MOVESTV_DESCR[ GetRandomInt( 0, ProductSeedData.MOVESTV_DESCR.Length - 1 ) ],
            5 => ProductSeedData.COURSE_DESCR[ GetRandomInt( 0, ProductSeedData.COURSE_DESCR.Length - 1 ) ],
            _ => string.Empty
        };
    }
    // XML
    string GetBookXml()
    {
        var author = new XElement( "Author", GetRandomSpec( ProductSeedData.NAMES ) );
        var publisher = new XElement( "Publisher", GetRandomSpec( ProductSeedData.PUBLISHERS ) );
        var isbn = new XElement( "ISBN", GetRandomSpec( ProductSeedData.ISBNS ) );
        var pages = new XElement( "Pages", GetRandomInt( 0, ProductSeedData.MAX_PAGE_LENGTH ).ToString() );
        
        var root = new XElement( "RootElement" );
        root.Add( author );
        root.Add( publisher );
        root.Add( isbn );
        root.Add( pages );

        if ( !GetRandomBoolean() ) 
            return root.ToString();
        
        var narrator = new XElement( "Narrator", GetRandomSpec( ProductSeedData.NAMES ) );
        var audioLength = new XElement( "AudioLength", GetRandomInt( 0, ProductSeedData.MAX_AUDIO_LENGTH ).ToString() );
        root.Add( narrator );
        root.Add( audioLength );

        return root.ToString();
    }
    string GetSoftwareXml()
    {
        var version = new XElement( "Version", GetRandomSpec( ProductSeedData.SOFTWARE_VERSIONS ) );
        var developer = new XElement( "Developer", GetRandomSpec( ProductSeedData.SOFTWARE_DEVELOPERS ) );
        var dependencies = new XElement( "Dependencies", GetRandomSpecs( ProductSeedData.SOFTWARE_DEPENDENCIES, 5 ) );
        var trialLimitations = new XElement( "TrialLimitations", GetRandomSpec( ProductSeedData.SOFTWARE_TRIAL_LIMITATIONS ) );

        var root = new XElement( "RootElement" );
        root.Add( version );
        root.Add( developer );
        root.Add( dependencies );
        root.Add( trialLimitations );

        return root.ToString();
    }
    string GetGameXml()
    {
        var developer = new XElement( "Developer", GetRandomSpec( ProductSeedData.GAME_DEVELOPERS ) );
        var hasOfflineMode = new XElement( "HasOfflineMode", GetRandomBoolean().ToString() );
        var hasInGamePurchases = new XElement( "HasInGamePurchases", GetRandomBoolean().ToString() );
        var hasControllerSupport = new XElement( "HasControllerSupport", GetRandomBoolean().ToString() );
        var fileSize = new XElement( "FileSizeMb", GetRandomInt( ProductSeedData.MIN_FILE_SIZE, ProductSeedData.MAX_FILE_SIZE ).ToString() );

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
        var director = new XElement( "Director", GetRandomSpec( ProductSeedData.DIRECTORS ) );
        var cast = new XElement( "Cast", GetRandomSpecs( ProductSeedData.NAMES, 10) );
        var runtimeMinutes = new XElement( "RuntimeMinutes", GetRandomInt( 1, ProductSeedData.MAX_RUNTIME ).ToString() );
        var episodes = new XElement( "Episodes", GetRandomInt( 1, ProductSeedData.MAX_EPISODES ).ToString() );
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
        var instructors = new XElement( "Instructors", GetRandomSpecs( ProductSeedData.NAMES, 3 ) );
        var requirements = new XElement( "Requirements", GetRandomSpecs( ProductSeedData.COURSE_REQUIREMENTS, 5 ) );
        var durationWeeks = new XElement( "DurationWeeks", GetRandomInt( 1, ProductSeedData.MAX_COURSE_DURATION ).ToString() );
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
        CategoryModel categoryModel = categoryData.CategoriesById[ currentCategory ];

        if ( categoryModel.Children.Count <= 0 )
            return new List<int>(); // Return an empty list instead of the original one

        List<int> subcategories = PickRandomCategories( categoryModel ).ToList();
        List<int> newPickedCategories = new List<int>( subcategories ); // Create a new list for this call

        foreach ( int category in subcategories )
        {
            // Merge the results from the recursive calls into the new list
            newPickedCategories.AddRange( GetRandomCategories( category, newPickedCategories, categoryData ) );
        }

        return newPickedCategories.Distinct(); // Remove duplicates before returning
    }
    IEnumerable<int> PickRandomCategories( CategoryModel category )
    {
        var picked = new HashSet<int>();

        int maxIndex = category.Children.Count - 1;
        int amount = GetRandomInt( 1, ProductSeedData.MAX_CATEGORIES );
        int count = 0;

        while ( picked.Count < amount )
        {
            int i = GetRandomInt( 0, maxIndex );
            int index = category.Children[ i ].CategoryId;

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
    // REVIEWS
    List<ProductReviewModel> GetRandomReviews( int category, IReadOnlyList<int> userIds )
    {
        List<ProductReviewModel> reviews = new();
        int amount = GetRandomInt( 1, 20 );

        for ( int i = 0; i < amount; i++ )
        {
            reviews.Add( new ProductReviewModel
            {
                UserId = userIds[ GetRandomInt( 0, userIds.Count - 1 ) ],
                Review = GetRandomSpec( ProductSeedData.PRODUCT_REVIEWS[ category - 1 ] ),
                Rating = GetRandomInt( 1, 5 )
            } );
        }

        return reviews;
    }
    // SPEC LOOKUP
    Dictionary<int, List<int>> GetRandomLookups( List<int> ids, SpecsResponse lookups )
    {
        Dictionary<int, List<int>> specs = new();

        foreach ( int i in ids )
        {
            Spec response = lookups.SpecsById[ i ];
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
    int PickRandomVendor( int category, VendorsResponse vendors )
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
        long randomTicks = ( long ) ( _random.NextDouble() * range ) + ProductSeedData.MIN_RELEASE_DATE.Ticks;

        // Convert back to DateTime
        return new DateTime( randomTicks );
    }
    int GetRandomInt( int min, int max )
    {
        if ( min >= max )
            throw new ArgumentOutOfRangeException( nameof( min ), "min must be less than max" );

        return _random.Next( min, max );
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