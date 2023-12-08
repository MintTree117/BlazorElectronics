using System.Reflection;
using BlazorElectronics.Client.Services.Admin;
using BlazorElectronics.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.Admin.Crud;

public class CrudPage<Tview, Tedit> : AdminPage where Tview : CrudView where Tedit : ICrudEdit, new()
{
    [Inject] protected IAdminCrudService<Tview, Tedit> CrudService { get; init; } = default!;

    const string ERROR_GET_VIEW = "Failed to load item view!";
    const string ERROR_GET_EDIT = "Failed to get item for edit!";

    public string PageTitle { get; private set; } = "CrudPage";
    protected string ItemTitle = "CrudItem";
    
    protected string ApiPath = string.Empty;

    protected List<Tview> ItemsView = new();
    protected readonly Dictionary<string,Action> THeadMeta = new();
    List<string> ColumnProperties = new();
    
    protected Tedit ItemEdit = default!;
    protected bool IsEditing;
    protected bool NewItem;

    public IReadOnlyList<Tview> GetView()
    {
        return ItemsView;
    }
    public IReadOnlyDictionary<string, Action> GetTHeadMeta()
    {
        return THeadMeta;
    }
    public IEnumerable<string> GetColumnProperties()
    {
        return ColumnProperties;
    }
    
    // INIT
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if ( !PageIsAuthorized )
            return;

        PageTitle = $"{ItemTitle} View";

        GenerateTableMeta();
    }
    protected async Task LoadView()
    {
        PageIsLoaded = false;

        ServiceReply<List<Tview>?> reply = await CrudService.GetView( $"{ApiPath}/get-view" );

        PageIsLoaded = true;

        if ( !reply.Success || reply.Data is null )
        {
            Logger.LogError( reply.Message ?? ERROR_GET_VIEW );
            SetViewMessage( false, reply.Message ?? ERROR_GET_VIEW );
            return;
        }

        ItemsView = reply.Data;

        SetViewMessage( true, string.Empty );
    }
    protected virtual void GenerateTableMeta()
    {
        THeadMeta.Add( "Id", SortById );
        THeadMeta.Add( "Name", SortByName );

        ColumnProperties = GetPropertiesInOrder( typeof( Tview ) );
    }
    static List<string> GetPropertiesInOrder( Type type )
    {
        var properties = new List<PropertyInfo>();
        CollectProperties( type, properties );
        return properties.Select( p => p.Name ).ToList();
    }
    static void CollectProperties( Type type, List<PropertyInfo> properties )
    {
        // Base case for recursion
        if ( type == null || type == typeof( object ) )
        {
            return;
        }

        // Recursively collect properties from the base class
        if ( type.BaseType != null )
            CollectProperties( type.BaseType, properties );

        // Add properties from the current type
        properties.AddRange( type.GetProperties( BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly ) );
    }
    
    // VIEW
    public object RenderPropertyColumn( Tview item, string propertyName )
    {
        PropertyInfo? propertyInfo = item.GetType().GetProperty( propertyName );
        return ( propertyInfo != null ? propertyInfo.GetValue( item ) : "" ) ?? "NULL VALUE";
    }
    public virtual void CreateItem()
    {
        PageTitle = $"Create {ItemTitle}";
        ItemEdit = new Tedit();
        IsEditing = true;
        NewItem = true;
    }
    public virtual async Task EditItem( int itemId )
    {
        PageIsLoaded = false;
        
        ServiceReply<Tedit?> reply = await CrudService.GetEdit( $"{ApiPath}/get-edit", new IntDto( itemId ) );

        if ( !reply.Success || reply.Data is null )
        {
            Logger.LogError( reply.Message ?? ERROR_GET_EDIT );
            SetViewMessage( false, reply.Message ?? ERROR_GET_EDIT );
            return;
        }

        ItemEdit = reply.Data;
        
        PageIsLoaded = true;
        IsEditing = true;

        PageTitle = GetEditTitle();
        
        StateHasChanged();
    }
    public async Task RemoveItem( int itemId )
    {
        ServiceReply<bool> result = await CrudService.RemoveItem( $"{ApiPath}/remove", new IntDto( itemId ) );

        if ( !result.Success )
        {
            SetActionMessage( false, $"Failed to delete item {itemId}. {result.Message}" );
            return;
        }

        SetActionMessage( true, $"Successfully deleted item {itemId}." );
        await LoadView();
        StateHasChanged();
    }
    void SortById()
    {
        ItemsView = ItemsView.OrderBy( c => c.Id ).ToList();
        StateHasChanged();
    }
    void SortByName()
    {
        ItemsView = ItemsView.OrderBy( c => c.Name ).ToList();
        StateHasChanged();
    }
    
    // EDIT
    protected virtual string GetEditTitle()
    {
        return $"Edit {ItemTitle}";
    }
    public void GoBack()
    {
        PageTitle = $"{ItemTitle} View";
        IsEditing = false;
        NewItem = false;
        StateHasChanged();
    }
    protected virtual async Task Submit()
    {
        if ( NewItem )
            await SubmitNew();
        else
            await SubmitUpdate();

        await LoadView();
    }
    async Task SubmitNew()
    {
        ServiceReply<int> reply = await CrudService.Add( $"{ApiPath}/add", ItemEdit );

        if ( !reply.Success )
        {
            SetActionMessage( false, reply.Message ?? "Failed to insert item, no response message!" );
            return;
        }
        
        ItemEdit.SetId( reply.Data );
        NewItem = false;
        
        SetActionMessage( true, "Successfully added item." );
        StateHasChanged();
    }
    async Task SubmitUpdate()
    {
        ServiceReply<bool> reply = await CrudService.Update( $"{ApiPath}/update", ItemEdit );

        if ( !reply.Success )
        {
            SetActionMessage( false, reply.Message ?? "Failed to update item, no response message!" );
            return;
        }

        SetActionMessage( true, "Successfully updated item." );
        StateHasChanged();
    }
}