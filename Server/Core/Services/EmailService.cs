using System.Net;
using System.Net.Mail;
using BlazorElectronics.Server.Api.Interfaces;

namespace BlazorElectronics.Server.Core.Services;

public class EmailService : _ApiService, IEmailService
{
    public EmailService( ILogger<EmailService> logger )
        : base( logger ) { }
    
    public void SendEmailAsync( string toEmail, string subject, string messageBody )
    {
        string? email = Environment.GetEnvironmentVariable( "AppEmail" );
        string? password = Environment.GetEnvironmentVariable( "AppEmailPassword" );

        if ( string.IsNullOrWhiteSpace( email ) )
            throw new Exception( "App email is null!" );

        if ( string.IsNullOrWhiteSpace( password ) )
            throw new Exception( "App email password is null!" );
        
        SmtpClient smtpClient = new( "smtp.gmail.com", 587 )
        {
            Credentials = new NetworkCredential( email, password ),
            EnableSsl = true
        };
        
        MailMessage mail = new()
        {
            From = new MailAddress( email ),
            Subject = subject,
            Body = messageBody,
            IsBodyHtml = true
        };
        
        mail.To.Add( new MailAddress( toEmail ) ); // userEmail is the recipient's email address
        smtpClient.SendAsync( mail, null );
    }
}