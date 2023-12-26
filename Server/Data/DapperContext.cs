using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Data;

public class DapperContext
{
    readonly ILogger<DapperContext> Logger;
    readonly string _connectionString;

    public DapperContext( ILogger<DapperContext> logger, IConfiguration config )
    {
        Logger = logger;
        _connectionString = config.GetConnectionString( "DefaultConnection" ) ?? string.Empty;
    }
    
    public async Task<SqlConnection> GetOpenConnection()
    {
        try
        {
            var connection = new SqlConnection( _connectionString );
            await connection.OpenAsync();
            return connection;
        }
        catch ( Exception e )
        {
            Logger.LogError( e.Message, e );
            return null;
        }
    }
}