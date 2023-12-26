namespace BlazorElectronics.Server.Api.Interfaces;

public interface IEmailService
{
    void SendEmailAsync( string toEmail, string subject, string messageBody );
}