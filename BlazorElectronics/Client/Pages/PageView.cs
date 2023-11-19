using System.Collections.Specialized;
using System.Web;

namespace BlazorElectronics.Client.Pages;

public abstract class PageView : RazorView
{
    protected const string ERROR_INVALID_URL_PARAMS = "Invalid url paramters!";
    protected const string PAGE_RETURN_URL_PARAM = "returnUrl";
    protected const string PAGE_RETURN_URL_DEFAULT = "";
    
    const int PAGE_REDIRECTION_WAIT_MILLISECONDS = 3000;

    protected string RazorViewMessage = "Loading Data...";
    protected string? ReturnUrl = string.Empty;

    protected bool PageIsLoaded = false;
    protected bool PageIsRedirecting = false;
    
    protected int PageRedirectCountdown = 3;
    protected System.Timers.Timer? PageRedirectTimer;

    protected virtual bool PageIsReady()
    {
        return PageIsLoaded;
    }
    protected void GetReturnUrl()
    {
        Uri uri = NavManager.ToAbsoluteUri( NavManager.Uri );
        NameValueCollection queryString = HttpUtility.ParseQueryString( uri.Query );
        ReturnUrl = queryString.Get( PAGE_RETURN_URL_PARAM );
    }
    protected void StartPageRedirection()
    {
        PageIsRedirecting = true;
        PageRedirectTimer = new System.Timers.Timer( PAGE_REDIRECTION_WAIT_MILLISECONDS );
        PageRedirectTimer.Elapsed += CountdownTimerElapsed;
        PageRedirectTimer.AutoReset = true;
        PageRedirectTimer.Enabled = true;
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