using BlazorElectronics.Client.Services.Admin;
using BlazorElectronics.Client.Services.Categories;
using BlazorElectronics.Client.Services.Specs;
using BlazorElectronics.Client.Services.Vendors;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Products;
using BlazorElectronics.Shared.Specs;
using BlazorElectronics.Shared.Vendors;
using Microsoft.AspNetCore.Components;

namespace BlazorElectronics.Client.Pages.Admin.ProductEdit;

public partial class EditProduct : AdminPage
{
    const string API_PATH = "api/AdminProduct";
    const string API_PATH_GET_EDIT = $"{API_PATH}/get-edit";
    const string API_PATH_ADD = $"{API_PATH}/add";
    const string API_PATH_UPDATE = $"{API_PATH}/update";
    const string API_PATH_REMOVE = $"{API_PATH}/remove";
    
    [Inject] public IAdminCrudService<CrudViewDto, ProductEditDto> ProductService { get; set; } = default!;
    [Inject] public IVendorServiceClient VendorService { get; set; } = default!;
    [Inject] public ICategoryServiceClient CategoryService { get; set; } = default!;
    [Inject] public ISpecServiceClient SpecService { get; set; } = default!;
    
    ProductEditDto _productDto = new();
    bool _isNew;
    bool _deleted;
    string _title = "Edit Product";

    VendorsDto _vendors = default!;
    CategoryDataDto _categories = default!;
    LookupSpecsDto _specs = default!;

    readonly Dictionary<int, bool> _selectedCategories = new();
    readonly Dictionary<(int specId, int specValueId), bool> _selectedSpecs = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if ( !PageIsAuthorized )
            return;
        
        if ( !ParseUrl( out int productId ) )
            return;

        if ( !_isNew )
            await LoadProductEdit( productId );

        PageIsLoaded = true;

        await Task.WhenAll( LoadVendors(), LoadCategories(), LoadSpecs() );
        
        InitializeCheckboxes();
        StateHasChanged();
    }

    bool ParseUrl( out int id )
    {
        id = -1;
        
        Uri uri = NavManager.ToAbsoluteUri( NavManager.Uri );
        Dictionary<string, string> queryParams = ParseQuery( uri.Query );
        queryParams.TryGetValue( Routes.PARAM_PRODUCT_EDIT_ID, out string? stringId );

        _isNew = string.IsNullOrWhiteSpace( stringId );
        _title = _isNew ? "Create Product" : "Edit Product";

        if ( _isNew )
            return true;
        
        if ( int.TryParse( stringId, out id ) )
            return true;

        SetViewMessage( false, "Invalid Url!" );
        return false;
    }

    async Task LoadProductEdit( int productId )
    {
        ServiceReply<ProductEditDto?> loadReply = await ProductService.GetEdit( API_PATH_GET_EDIT, productId );

        if ( !loadReply.Success || loadReply.Payload is null )
        {
            SetViewMessage( false, $"Failed to load product! {loadReply.ErrorType} : {loadReply.Message}" );
            return;
        }

        _productDto = loadReply.Payload;
    }
    async Task LoadVendors()
    {
        ServiceReply<VendorsDto?> reply = await VendorService.GetVendors();

        if ( reply.Payload is not null )
            _vendors = reply.Payload;
    }
    async Task LoadCategories()
    {
        ServiceReply<CategoryDataDto?> reply = await CategoryService.GetCategories();

        if ( reply.Payload is not null )
            _categories = reply.Payload;

        foreach ( int id in _categories.CategoriesById.Keys )
        {
            _selectedCategories.Add( id, false );
        }
    }
    async Task LoadSpecs()
    {
        ServiceReply<LookupSpecsDto?> reply = await SpecService.GetSpecLookups();

        if ( reply.Payload is not null )
            _specs = reply.Payload;

        foreach ( LookupSpec s in _specs.SpecsById.Values )
        {
            for ( int i = 0; i < s.Values.Count; i++ )
            {
                _selectedSpecs.Add( ( s.SpecId, i ), false );
            }
        }
    }

    void InitializeCheckboxes()
    {
        foreach ( int cid in _productDto.Categories )
        {
            _selectedCategories[ cid ] = true;
        }

        foreach ( KeyValuePair<int, List<int>> s in _productDto.LookupSpecs )
        {
            foreach ( int sv in s.Value )
            {
                _selectedSpecs[ ( s.Key, sv ) ] = true;
            }
        }
    }
    void SetDtoValuesFromCheckboxes()
    {
        _productDto.Categories = new List<int>();
        _productDto.LookupSpecs = new Dictionary<int, List<int>>();

        foreach ( int id in _selectedCategories.Keys.Where( id => _selectedCategories[ id ] ) )
            _productDto.Categories.Add( id );

        foreach ( (int specId, int specValueId) tuple in _selectedSpecs.Keys.Where( tuple => _selectedSpecs[ tuple ] ) )
        {
            if ( !_productDto.LookupSpecs.TryGetValue( tuple.specId, out List<int>? valuesList ) )
            {
                valuesList = new List<int>();
                _productDto.LookupSpecs.Add( tuple.specId, valuesList );
            }

            valuesList.Add( tuple.specValueId );
        }
    }

    async Task Submit()
    {
        SetDtoValuesFromCheckboxes();
        
        if ( _isNew )
            await SubmitNew();
        else
            await SubmitEdit();
    }
    async Task SubmitNew()
    {
        ServiceReply<int> reply = await ProductService.Add( API_PATH_ADD, _productDto );

        if ( !reply.Success )
        {
            InvokeAlert( AlertType.Danger, $"Failed to submit new product! {reply.ErrorType} : {reply.Message}" );
            return;
        }

        _isNew = false;
        _title = "Edit Product";
        _productDto.ProductId = reply.Payload;
        InvokeAlert( AlertType.Success, $"Created product." );
        StateHasChanged();
    }
    async Task SubmitEdit()
    {
        ServiceReply<bool> reply = await ProductService.Update( API_PATH_UPDATE, _productDto );

        if ( !reply.Success )
        {
            InvokeAlert( AlertType.Danger, $"Failed to submit changes! {reply.ErrorType} : {reply.Message}" );
            return;
        }

        InvokeAlert( AlertType.Success, $"Updated product." );
        StateHasChanged();
    }
    async Task Remove()
    {
        ServiceReply<bool> reply = await ProductService.RemoveItem( API_PATH_REMOVE, _productDto.ProductId );

        if ( !reply.Success )
        {
            InvokeAlert( AlertType.Danger, $"Failed to delete product! {reply.ErrorType} : {reply.Message}" );
            return;
        }

        _deleted = true;
        StateHasChanged();
        InvokeAlert( AlertType.Success, $"Deleted product." );
    }
}