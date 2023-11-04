using System.Text;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server;

public static class LoggerUtility
{
    public static string CreateSqlMessage( string message, SqlException sqlException )
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
    public static string CreateDotnetMessage( string message, Exception exception )
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine( message );
        stringBuilder.AppendLine( "Exception Information:" );
        stringBuilder.AppendLine( $"Message: {exception.Message}" );
        stringBuilder.AppendLine( $"Source: {exception.Source}" );
        stringBuilder.AppendLine( $"StackTrace: {exception.StackTrace}" );

        if ( exception.TargetSite != null )
        {
            stringBuilder.AppendLine( $"TargetSite: {exception.TargetSite}" );
        }

        if ( exception.InnerException != null )
        {
            stringBuilder.AppendLine( "Inner Exception:" );
            stringBuilder.AppendLine( CreateDotnetMessage( "Inner Exception", exception.InnerException ) );
        }

        if ( exception.Data.Count > 0 )
        {
            stringBuilder.AppendLine( "Data:" );
            foreach ( var key in exception.Data.Keys )
            {
                stringBuilder.AppendLine( $"{key}: {exception.Data[ key ]}" );
            }
        }

        return stringBuilder.ToString();
    }
}