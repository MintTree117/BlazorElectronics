using System.Reflection;
using BlazorElectronics.Client.Services.Users.Admin;
using BlazorElectronics.Shared;

namespace BlazorElectronics.Client.Pages.User.Admin;

public class AdminView<T> : AdminPage where T : AdminItemViewDto
{
    protected IAdminViewService<T> ViewService;

    const string ERROR_GET_VIEW = "Failed to load item view!";
    const string UrlNew = "newItem";
    const string UrlId = "itemId";

    protected string UrlItemName = string.Empty;
    protected List<T> Items = new();
    protected List<THeadDisplayMeta> THeadMeta = new();
    protected List<string> TPropertyNames = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        THeadMeta = GenerateTHeadMeta();
        TPropertyNames = GenerateTBodyMeta();
    }

    void SortById()
    {
        Items = Items.OrderBy( c => c.Id ).ToList();
        StateHasChanged();
    }
    void SortByName()
    {
        Items = Items.OrderBy( c => c.Name ).ToList();
        StateHasChanged();
    }

    protected void CreateItem()
    {
        NavManager.NavigateTo( $"admin/{UrlItemName}/edit?{UrlNew}=true" );
    }
    protected void EditItem( int itemId )
    {
        NavManager.NavigateTo( $"admin/{UrlItemName}/edit?{UrlNew}=false&{UrlId}={itemId}" );
    }
    protected async Task LoadView()
    {
        PageIsLoaded = false;

        ServiceReply<List<T>?> reply = await ViewService.GetView();

        PageIsLoaded = true;

        if ( !reply.Success || reply.Data is null )
        {
            Logger.LogError( reply.Message ?? ERROR_GET_VIEW );
            SetViewMessage( false, reply.Message ?? ERROR_GET_VIEW );
            return;
        }

        Items = reply.Data;

        ViewMessage = string.Empty;
    }
    protected async Task RemoveItem( int itemId )
    {
        ServiceReply<bool> result = await ViewService.RemoveItem( new IntDto( itemId ) );

        if ( !result.Success )
        {
            SetActionMessage( false, $"Failed to delete item {itemId}. {result.Message}" );
            return;
        }

        SetActionMessage( true, $"Successfully deleted item {itemId}." );
        await LoadView();
        StateHasChanged();
    }

    protected virtual List<THeadDisplayMeta> GenerateTHeadMeta()
    {
        return new List<THeadDisplayMeta>
        {
            new THeadDisplayMeta( "Id", SortById ),
            new THeadDisplayMeta( "Name", SortByName )
        };
    }
    protected object RenderPropertyColumn( object item, string propertyName )
    {
        PropertyInfo? propertyInfo = item.GetType().GetProperty( propertyName );
        return ( propertyInfo != null ? propertyInfo.GetValue( item ) : "" ) ?? "NULL VALUE";
    }
    static List<string> GenerateTBodyMeta()
    {
        return typeof( T ).GetProperties()
            .Select( prop => prop.Name )
            .ToList();
    }
}