using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Data;

public class DapperContext
{
    readonly IConfiguration _config;
    readonly string _connectionString;

    public DapperContext( IConfiguration config )
    {
        _config = config;
        _connectionString = config.GetConnectionString( "DefaultConnection" ) ?? string.Empty;
    }

    public SqlConnection CreateConnection() => new SqlConnection( _connectionString );
    public async Task<SqlConnection> GetOpenConnection()
    {
        var connection = new SqlConnection( _connectionString );
        await connection.OpenAsync();
        return connection;
    }
}