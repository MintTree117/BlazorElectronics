using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Data;

public class DapperContext
{
    ILogger<DapperContext> Logger;
    readonly IConfiguration _config;
    readonly string _connectionString;

    public DapperContext( IConfiguration config, ILogger<DapperContext> logger )
    {
        _config = config;
        Logger = logger;
        _connectionString = config.GetConnectionString( "DefaultConnection" ) ?? string.Empty;
    }
    
    public async Task<SqlConnection> GetOpenConnection()
    {
        try
        {
            Logger.LogWarning( "COnn -------------------------------------------------- : " + _connectionString );
            Logger.LogError( "COnn -------------------------------------------------------- : " + _connectionString );
            Logger.LogWarning( "COnn -------------------------------------------------- : " + _connectionString );
            var connection = new SqlConnection( _connectionString );
            await connection.OpenAsync();
            return connection;
        }
        catch ( Exception e )
        {
            Logger.LogError( $"----------------------------------------- STRING---------- {_connectionString} e.Message ", e );
            return null;
        }
    }
}