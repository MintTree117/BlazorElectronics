namespace BlazorElectronics.Client.Pages.User.Admin;

public class THeadDisplayMeta
{
    public THeadDisplayMeta( string name, Action action )
    {
        DisplayName = name;
        SortByDelegate = action;
    }

    public string DisplayName { get; set; }
    public readonly Action SortByDelegate;
}