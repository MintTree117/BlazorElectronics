using BlazorElectronics.Server.Admin.Models.Products;
using BlazorElectronics.Server.Admin.Services;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Dtos.Categories;
using BlazorElectronics.Server.Dtos.Specs;
using BlazorElectronics.Server.Repositories;

namespace BlazorElectronics.Server.Admin.Repositories;

public sealed class AdminProductDummyRepository : DapperRepository, IAdminProductDummyInsertRepository
{
    readonly Random _random = new();
    
    public AdminProductDummyRepository( DapperContext dapperContext )
        : base( dapperContext ) { }
    
    public async Task<bool> Insert( int amount, CategoriesDto categoryData, CachedSpecData specLookupData )
    {
        throw new NotImplementedException();
    }

    List<T> GetBaseModels<T>( int amount, int category ) where T : AdminProductDummyModel, new()
    {
        var models = new List<T>();
        
        for ( int i = 0; i < amount; i++ )
        {
            var product = new T();

            product.Title = AdminProductDummyData.PRODUCT_TITLES[ category ] + " " + i;
            product.ThumbnailUrl = AdminProductDummyData.PRODUCT_IMAGES[ category ];
            product.ReleaseDate = GetRandomDate();
            product.NumberSold = GetRandomSold();
            product.HasDrm = GetRandomBoolean();
        }

        return models;
    }

    DateTime GetRandomDate()
    {
        // Convert to ticks
        long range = AdminProductDummyData.MAX_RELEASE_DATE.Ticks - AdminProductDummyData.MIN_RELEASE_DATE.Ticks;

        // Create a random range between them
        long randomTicks = ( long ) ( _random.NextDouble() * range ) + AdminProductDummyData.MIN_RELEASE_DATE.Ticks;

        // Convert back to DateTime
        return new DateTime( randomTicks );
    }
    int GetRandomInt( int min, int max )
    {
        // Generate a random double number between 0 and 1
        double randomDouble = _random.NextDouble();

        // Adjust the range of the double number
        double range = ( double ) ( AdminProductDummyData.MAX_NUM_SOLD );
        double randomInRange = ( randomDouble * range );

        // Convert to decimal and return
        return ( int ) randomInRange;
    }
    int GetRandomSold()
    {
        // Generate a random double number between 0 and 1
        double randomDouble = _random.NextDouble();

        // Adjust the range of the double number
        double range = ( double ) ( AdminProductDummyData.MAX_NUM_SOLD );
        double randomInRange = ( randomDouble * range );

        // Convert to decimal and return
        return ( int ) randomInRange;
    }
    decimal GetRandomPrice()
    {
        // Generate a random double number between 0 and 1
        double randomDouble = _random.NextDouble();

        // Adjust the range of the double number
        double range = ( double ) ( AdminProductDummyData.MAX_PRICE );
        double randomInRange = ( randomDouble * range );

        // Convert to decimal and return
        return ( decimal ) randomInRange;
    }
    bool GetRandomBoolean()
    {
        // Generate a random integer (0 or 1) and convert it to a boolean
        return _random.Next( 2 ) == 1;
    }
}