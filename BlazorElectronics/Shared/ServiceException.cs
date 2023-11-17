using System.Text;

namespace BlazorElectronics.Shared;

public class ServiceException : Exception
{
    public ServiceException( string message, Exception? exception )
        : base( CreateExceptionMessage( message, exception ), exception ) { }
    
    protected static string CreateExceptionMessage( string message, Exception? exception )
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine( message );

        if ( exception == null )
            return stringBuilder.ToString();
        
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
            stringBuilder.AppendLine( CreateExceptionMessage( "Inner Exception", exception.InnerException ) );
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