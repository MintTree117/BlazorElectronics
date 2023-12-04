using System.Collections.Specialized;
using System.Web;

namespace BlazorElectronics.Client.Pages;

public abstract class PageView : RazorView
{
    protected bool PageIsLoaded = false;
    public bool PageIsRedirecting { get; private set; }
    
    const string MESSAGE_SUCCESS_CLASS = "text-success";
    const string MESSAGE_FAILURE_CLASS = "text-danger";
    public string ViewMessageClass { get; private set; } = MESSAGE_FAILURE_CLASS;
    public string ActionMessageClass { get; private set; } = MESSAGE_FAILURE_CLASS;
    public string ViewMessage { get; private set; } = "Loading Data...";
    public string ActionMessage { get; private set; } = string.Empty;
    
    const int PAGE_REDIRECTION_WAIT_MILLISECONDS = 3000;
    const string PAGE_RETURN_URL_PARAM = "returnUrl";
    const string PAGE_RETURN_URL_DEFAULT = "";
    
    string? ReturnUrl = string.Empty;
    protected int PageRedirectCountdown { get; private set; } = 3;
    System.Timers.Timer? PageRedirectTimer;
    
    public virtual bool PageIsReady()
    {
        return PageIsLoaded;
    }
    protected void GetReturnUrl()
    {
        Uri uri = NavManager.ToAbsoluteUri( NavManager.Uri );
        NameValueCollection queryString = HttpUtility.ParseQueryString( uri.Query );
        ReturnUrl = queryString.Get( PAGE_RETURN_URL_PARAM );
    }
    protected void StartPageRedirection( string message )
    {
        SetViewMessage( false, message );
        PageIsRedirecting = true;
        PageRedirectTimer = new System.Timers.Timer( PAGE_REDIRECTION_WAIT_MILLISECONDS );
        PageRedirectTimer.Elapsed += CountdownTimerElapsed;
        PageRedirectTimer.AutoReset = true;
        PageRedirectTimer.Enabled = true;
    }
    protected void SetViewMessage( bool success, string message )
    {
        ViewMessageClass = success ? MESSAGE_SUCCESS_CLASS : MESSAGE_FAILURE_CLASS;
        ViewMessage = message;
    }
    protected void SetActionMessage( bool success, string message )
    {
        ActionMessageClass = success ? MESSAGE_SUCCESS_CLASS : MESSAGE_FAILURE_CLASS;
        ActionMessage = message;
    }
    
    void CountdownTimerElapsed( object? sender, System.Timers.ElapsedEventArgs e )
    {
        if ( PageRedirectCountdown > 0 )
        {
            PageRedirectCountdown--;
            StateHasChanged();
            return;
        }
        
        PageRedirectTimer?.Stop();
        PageRedirectTimer?.Dispose();
        
        string returnUrl = !string.IsNullOrWhiteSpace( ReturnUrl ) ? ReturnUrl : PAGE_RETURN_URL_DEFAULT;
        NavManager.NavigateTo( returnUrl );
    }
}