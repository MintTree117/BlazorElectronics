using System.Data;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Repositories;
namespace BlazorElectronics.Server.Admin.Repositories;

public class _AdminRepository : DapperRepository
{
    protected _AdminRepository( DapperContext dapperContext ) : base( dapperContext ) { }
    
    protected static DataTable GetPrimaryCategoriesTable( string categoriesString )
    {
        List<string> categoryStrings = categoriesString
            .Split( ',' )
            .Select( s => s.Trim() ) // Trims whitespace from each item.
            .ToList();

        var categories = new List<int>();

        foreach ( string c in categoryStrings )
        {
            if ( int.TryParse( c, out int category ) )
                categories.Add( category );
        }

        var table = new DataTable();
        table.Columns.Add( COL_CATEGORY_PRIMARY_ID, typeof( int ) );

        foreach ( int id in categories )
            table.Rows.Add( id );

        return table;
    }
    protected static DataTable GetStringValuesTable( string valuesString, string idCol, string valueCol )
    {
        List<string> values = valuesString
            .Split( ',' )
            .Select( s => s.Trim() ) // Trims whitespace from each item.
            .ToList();

        var table = new DataTable();

        table.Columns.Add( idCol, typeof( int ) );
        table.Columns.Add( valueCol, typeof( string ) );

        for ( int i = 0; i < values.Count; i++ )
        {
            DataRow row = table.NewRow();
            row[ idCol ] = i + 1;
            row[ valueCol ] = values[ i ];
            table.Rows.Add( row );
        }

        return table;
    }
}