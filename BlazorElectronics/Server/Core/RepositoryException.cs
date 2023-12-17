using System.Text;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Core;

public sealed class RepositoryException : ServiceException
{
    public RepositoryException( string message, SqlException sqlException )
        : base( CreateSqlExceptionMessage( message, sqlException ), sqlException ) { }
    
    public RepositoryException( string message, Exception exception )
        : base( CreateExceptionMessage( message, exception ), exception ) { }

    static string CreateSqlExceptionMessage( string message, SqlException sqlException )
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine( message );
        stringBuilder.AppendLine( "SQL Exception Information:" );
        stringBuilder.AppendLine( $"Message: {sqlException.Message}" );
        stringBuilder.AppendLine( $"Number: {sqlException.Number}" );
        stringBuilder.AppendLine( $"State: {sqlException.State}" );
        stringBuilder.AppendLine( $"Class: {sqlException.Class}" );
        stringBuilder.AppendLine( $"Server: {sqlException.Server}" );
        stringBuilder.AppendLine( $"Procedure: {sqlException.Procedure}" );
        stringBuilder.AppendLine( $"LineNumber: {sqlException.LineNumber}" );

        // Loop over the errors (a single SQL Exception can contain multiple errors)
        foreach ( SqlError error in sqlException.Errors )
        {
            stringBuilder.AppendLine( "Error Information:" );
            stringBuilder.AppendLine( $"Message: {error.Message}" );
            stringBuilder.AppendLine( $"Source: {error.Source}" );
            stringBuilder.AppendLine( $"State: {error.State}" );
            stringBuilder.AppendLine( $"Class: {error.Class}" );
            stringBuilder.AppendLine( $"Server: {error.Server}" );
            stringBuilder.AppendLine( $"Procedure: {error.Procedure}" );
            stringBuilder.AppendLine( $"LineNumber: {error.LineNumber}" );
        }

        return stringBuilder.ToString();
    }
}